using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface IBackupRestoreRepository : IRepository<OrigamiBackup>
    {
        /// <summary>
        /// Backup/restore task
        /// </summary>
        Task? BackupRestoreTask { get; set; }

        /// <summary>
        /// Current backup or restore process
        /// </summary>
        OrigamiBackup? Current { get; set; }

        /// <summary>
        /// Starts a backup for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<OrigamiBackup>> BackupAsync(OrigamiUser user);

        /// <summary>
        /// Starts a restore for the specified backup
        /// </summary>
        /// <param name="user"></param>
        /// <param name="backup"></param>
        /// <param name="filepathOverride"></param>
        /// <returns></returns>
        Task<Result<OrigamiBackupRestore>> RestoreAsync(OrigamiUser user, OrigamiBackup backup, string? filepathOverride = null);
    }
}
