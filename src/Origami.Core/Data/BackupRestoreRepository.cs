using FluentValidation;
using Lucene.Net.Util;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;

namespace Origami.Core.Data
{
    public class BackupRestoreRepository : 
        RepositoryOuterLayer<OrigamiBackup>, 
        IBackupRestoreRepository
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IConfiguration _configuration;
        protected readonly IFileRepository _fileRepository;
        protected readonly IUserRepository _userRepository;

        public BackupRestoreRepository(
            IAppFacade appFacade,
            IConfiguration configuration,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IFileRepository fileRepository,
            IMemoryCache memoryCache,
            IUserRepository userRepository,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _appFacade = appFacade;
            _configuration = configuration;
            _fileRepository = fileRepository;
            _userRepository = userRepository;
        }

        public Task? BackupRestoreTask { get; set; }

        public OrigamiBackup? Current { get; set; }

        public override string ReadPermission => nameof(OrigamiRole.ViewBackupRestoreSystem);

        /// <summary>
        /// New database name from the current process
        /// </summary>
        private string DatabaseName => $"origami-{Current?.NanoId}";

        public async Task<Result<OrigamiBackup>> Backup(OrigamiUser user)
        {
            if (Current != null)
            {
                return new() { ErrorMessage = Text.Original("A backup or restore process is already running. Please try again later.") };
            }

            var hub = new Result<string>();
            var backup = new OrigamiBackup { UserId = user.Id, DateCreated = DateTime.UtcNow };

            try
            {
                if (Directory.Exists(WebRootPath.WebRootPathForBackups) == false)
                {
                    Directory.CreateDirectory(WebRootPath.WebRootPathForBackups);
                }

                Current = backup.Clone();

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Backup);

                hub = await this.BackupTheDatabaseAsync();
                if (hub.Ok == false)
                {
                    return new Result<OrigamiBackup>(backup).Pull(hub);
                }

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Backup);

                string sourceFolder = $"{WebRootPath.WebRootPath}/files/";
                string zipPath = $"{WebRootPath.WebRootPathForBackups}/{Current.Filename}";

                await ZipFile.CreateFromDirectoryAsync(
                    sourceFolder,
                    zipPath,
                    CompressionLevel.Optimal,
                    includeBaseDirectory: true
                );

                hub.SuccessMessage = Text.Original("ZIP file created successfully.");

                if (hub.Ok)
                {
                    var ctx = Current.GetContext(user);
                    this.SmartSave(ctx, false).Push(hub);
                }

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Backup);

                return new Result<OrigamiBackup>(backup).Pull(hub);
            }
            catch (Exception ex)
            {
                return new() { ErrorMessage = ex.GetMessage() };
            }
            finally
            {
                Current = null;
                _appFacade.RefreshUI(OrigamiConstants.Events.BackupComplete, hub);
            }
        }

        public async Task<Result<OrigamiBackupRestore>> Restore(OrigamiUser user, OrigamiBackup backup)
        {
            if (Current != null)
            {
                return new() { ErrorMessage = Text.Original("A backup or restore process is already running. Please try again later.") };
            }

            if (backup is OrigamiBackupRestore)
            {
                return new() { ErrorMessage = Text.Original("This has already been restored.") };
            }

            var hub = new Result();
            var restore = new OrigamiBackupRestore() { UserId = user.Id, DateCreated = DateTime.UtcNow };

            try
            {
                if (Directory.Exists(WebRootPath.WebRootPathForRestores) == false)
                {
                    Directory.CreateDirectory(WebRootPath.WebRootPathForRestores);
                }

                var zipPath = Path.Combine(WebRootPath.WebRootPathForBackups, $"{backup.NanoId}.zip");
                var extractPath = Path.Combine(WebRootPath.WebRootPathForRestores, backup.NanoId);

                if (Directory.Exists(extractPath) == true)
                {
                    Directory.Delete(extractPath, true);
                }
                
                Current = restore.Clone();

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Restore);

                if (File.Exists(zipPath) == false)
                {
                    return new(restore) { ErrorMessage = Text.Original("Backup file not found.") };
                }

                await ZipFile.ExtractToDirectoryAsync(zipPath, extractPath);

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Restore);

                hub = await RestoreTheDatabaseAsync(Path.Combine(extractPath, "files", "db.bacpac"));
                if (hub.Ok == false)
                {
                    return new Result<OrigamiBackupRestore>(restore).Pull(hub);
                }

                //update connection string inside appsettings.json
                var cs = await UpdateConnectionStringInsideDbSettings();

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Restore);

                //pushes the result to hub
                cs.Push(hub);

                if (hub.Ok == false)
                {
                    return new Result<OrigamiBackupRestore>(restore).Pull(hub);
                }

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Restore);

                //rename current files folder to files_old_{CurrentProcess.NanoId}
                Directory.Move($"{WebRootPath.WebRootPath}/files/", $"{WebRootPath.WebRootPath}/files-old-{Current.NanoId}/");
                Directory.Move($"{extractPath}/files/", $"{WebRootPath.WebRootPath}/files/");

                //asks to refresh the UI
                _appFacade.RefreshUI(OrigamiConstants.Events.Restore);

                //save the restore record
                if (hub.Ok)
                {
                    var ctx = Current.GetContext(user);
                    this.SmartSave(ctx, false).Push(hub);
                }

                return new Result<OrigamiBackupRestore>(restore).Pull(hub);
            }
            catch (Exception ex)
            {
                return new() { ErrorMessage = ex.GetMessage() };
            }
            finally
            {
                Current = null;
                _appFacade.RefreshUI(OrigamiConstants.Events.RestoreComplete, hub);
            }
        }

        public override Result<OrigamiBackup> SmartPurge(DataOperationContext<OrigamiBackup> ctx, bool checkPermission)
        {
            var hub = base.SmartPurge(ctx, false);

            if (hub.Ok)
            {
                try
                {
                    var filepath = $"{WebRootPath.WebRootPath}/backups/{ctx.Entity.NanoId}.zip";
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                    var bppath = $"{WebRootPath.WebRootPath}/backups/{ctx.Entity.NanoId}.bacpac";
                    if (File.Exists(bppath))
                    {
                        File.Delete(bppath);
                    }
                }
                catch (Exception ex)
                {
                    return new() { ErrorMessage = ex.GetMessage() };
                }
            }

            return hub;
        }

        protected async Task<Result<string>> BackupTheDatabaseAsync()
        {
            if (Current == null)
            {
                return new() { ErrorMessage = $"Current process hasn't started yet" };
            }

            var oi = _configuration.GetOrigamiConnectionString();
            var target = $"{WebRootPath.WebRootPath}/files/db.bacpac";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sqlpackage",
                    Arguments = $"/Action:Export " +
                    $"/SourceConnectionString:\"{oi}\" " +
                    $"/TargetFile:\"{target}\" " + 
                    $"/OverwriteFiles:True",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                return new() { ErrorMessage = $"BACPAC export failed: {error}" };
            }

            return new(target) { SuccessMessage = Text.Original("BACPAC file created successfully.") };
        }

        protected async Task<Result> RestoreTheDatabaseAsync(string bacpacPath)
        {
            if (File.Exists(bacpacPath) == false)
            {
                return new() { ErrorMessage = "BACPAC file not found." };
            }

            if (Current == null)
            {
                return new() { ErrorMessage = $"Current process hasn't started yet" };
            }

            var oi = _configuration.GetOrigamiConnectionString();
            var builder = new SqlConnectionStringBuilder(oi);

            var args = $"/Action:Import " +
                $"/SourceFile:\"{bacpacPath}\" " +
                $"/TargetServerName:\"{builder.DataSource}\" " +
                $"/TargetDatabaseName:\"{this.DatabaseName}\" " +
                $"/TargetUser:\"origami-backup\" " +
                $"/TargetPassword:\"zwREK18C7kDDoLXREGWs\" " +
                $"/TargetEncryptConnection:False";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sqlpackage",
                    Arguments = args.Replace("\n", " "),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"BACPAC import failed:\n{error}");
            }

            return new() { SuccessMessage = Text.Original("Database restored successfully."), };
        }

        private async Task<Result> UpdateConnectionStringInsideDbSettings()
        {
            try
            {
                var oi = _configuration.GetOrigamiConnectionString();
                var builder = new SqlConnectionStringBuilder(oi)
                {
                    InitialCatalog = this.DatabaseName,
                };

                var path = Path.GetFullPath("..\\Origami.Files\\");
                var file = Path.Combine(path, $"dbsettings.{_appFacade.EnvironmentName}.json");
                file = File.Exists(file) ? file : Path.Combine(path, "dbsettings.json");

                var json = await File.ReadAllTextAsync(file);
                var node = JsonNode.Parse(json);

                if (node != null)
                {
                    node["ConnectionStrings"]!["origami"] = builder.ToString();
                    await File.WriteAllTextAsync(file, node.ToJsonString(new() { WriteIndented = true, }));
                    return new() { SuccessMessage = Text.Original("Db settings file updated successfully..") };
                }

                return new() { ErrorMessage = Text.Original("Error parsing the JSON file.") };
            }
            catch (Exception ex)
            {
                return new() { ErrorMessage = ex.GetMessage() };
            }
        }
    }
}
