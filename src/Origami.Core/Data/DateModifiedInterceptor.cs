using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public sealed class DateModifiedInterceptor : SaveChangesInterceptor
    {
        private static readonly TimeProvider TimeProvider = TimeProvider.System;

        private void UpdateFields(DbContext? context)
        {
            if (context is null) return;

            var now = TimeProvider.GetUtcNow().UtcDateTime;

            foreach (var entry in context.ChangeTracker.Entries<IDateModified>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.DateModified = now;
                        break;
                }
            }
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateFields(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateFields(eventData.Context);

            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }
    }
}
