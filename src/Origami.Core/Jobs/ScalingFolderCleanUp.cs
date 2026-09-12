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
                System.IO.Directory.Delete(scalingPath);
                System.IO.Directory.CreateDirectory(scalingPath);
            }

            return ValueTask.CompletedTask;
        }
    }
}
