using Origami.Core.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Jobs
{
    public class ScalingFolderCleanUp(ISuperRepository Super) : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var scaling = Super.Blogs.DirectoryForScalingImages();
            var scalingPath = Super.Directories.LocalPath(scaling);

            lock (OrigamiConstants.SyncRoot)
            {
                if (System.IO.Directory.Exists(scalingPath))
                {
                    System.IO.Directory.Delete(scalingPath, recursive: true);
                }
                System.IO.Directory.CreateDirectory(scalingPath);
            }

            return ValueTask.CompletedTask;
        }
    }
}
