using Origami.Core.Data;
using Origami.Core.Models.FileSystem;
using Quartz;

namespace Origami.Core.Jobs
{
    [DisallowConcurrentExecution]
    public class ScalingFolderCleanUp(ISuperRepository Super) : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var sizes = Enum.GetValues<ePictureSizes>();
            var scalingFiles = new List<string>();
            var scalingPath = Super.Directories.LocalPath(OrigamiSystemDirectory.DirectoryForScalingImages());
            var localPath = Super.Directories.LocalPath("/files/");

            Directory.CreateDirectory(scalingPath);

            var files = Directory.EnumerateFiles(localPath, "*", SearchOption.AllDirectories)
                .Select(filename => new OrigamiSystemFile(filename))
                .Where(file => file.IsImage)
                .ToList();

            foreach (var file in files)
            {
                foreach (var size in sizes)
                {
                    var scaledFilename = file.ScaleFilename(size);
                    var scaledFilePath = Path.Combine(scalingPath, scaledFilename);
                    if (File.Exists(scaledFilePath))
                    {
                        scalingFiles.Add(scaledFilePath);
                    }
                }
            }

            var query = from file in Directory.EnumerateFiles(scalingPath, "*", SearchOption.AllDirectories)
                        where !scalingFiles.Contains(file)
                        select file;

            foreach (var file in query)
            {
                File.Delete(file);
            }

            return ValueTask.CompletedTask;
        }
    }
}
