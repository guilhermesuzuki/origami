using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public interface IDatabaseMigration
    {
        /// <summary>
        /// Date and time this migration should be applied. This is used to determine the order of migrations and to check if a migration has already been applied.
        /// </summary>
        DateTime Key { get; }
        /// <summary>
        /// Determines whether the operation or change has already been applied.
        /// </summary>
        /// <returns>true if the operation has been applied; otherwise, false.</returns>
        bool HasBeenApplied();
        /// <summary>
        /// Marks the operation or change as applied.
        /// </summary>
        void MarkAsApplied();
        /// <summary>
        /// Executes the migration process to apply any pending changes to the underlying data store.
        /// </summary>
        /// <remarks>Call this method to ensure that the data store schema is up to date with the current
        /// application model. This operation may modify the structure of the data store and should typically be
        /// performed during application startup or deployment. The specific behavior depends on the
        /// implementation.</remarks>
        void Migrate();
    }
}
