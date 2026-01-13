using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface IBackupRestoreRepository : IRepository<OrigamiBackup>
    {
        /// <summary>
        /// Current backup or restore process
        /// </summary>
        OrigamiBackup? CurrentProcess { get; set; }

        /// <summary>
        /// Starts a backup for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<OrigamiBackup>> Backup(OrigamiUser user);
    }
}
