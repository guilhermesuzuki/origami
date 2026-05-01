using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data.Migrations
{
    public class DbMigrationRepository
    {
        protected readonly IEnumerable<DbMigration> _migrations;
        protected readonly ISuperRepository _superRepository;

        public DbMigrationRepository(IEnumerable<DbMigration> migrations, ISuperRepository superRepository)
        {
            _migrations = migrations;
            _superRepository = superRepository;
        }

        public bool Migrate()
        {
            var settings = _superRepository.Settings.GetSettings();

            var pendingMigrations = settings.LastDatabaseMigration != DateTime.MinValue
                ? _migrations.Where(x => x.Key > settings.LastDatabaseMigration).OrderBy(x => x.Key).ToList()
                : _migrations.OrderBy(x => x.Key).ToList();

            foreach (var migration in pendingMigrations)
            {
                migration.Migrate();
                if (migration.HasBeenApplied() == true)
                {
                    migration.MarkAsApplied();
                }
            }

            return false;
        }
    }
}
