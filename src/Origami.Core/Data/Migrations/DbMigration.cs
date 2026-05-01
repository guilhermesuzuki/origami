using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Origami.Core.Data.Migrations
{
    public abstract class DbMigration : IDatabaseMigration
    {
        protected readonly ISuperRepository _superRepository;
        protected DbMigration(ISuperRepository superRepository)
        {
            _superRepository = superRepository;
        }

        public abstract DateTime Key { get; }
        public abstract bool HasBeenApplied();
        public abstract void Migrate();

        public void MarkAsApplied()
        {
            using var tr = new TransactionScope();
            using var db = _superRepository.DbContextFactory.CreateDbContext();

            var v = new SqlParameter("@v", Key.ToString("yyyy-MM-dd HH:mm:ss"));
            var n = new SqlParameter("@n", nameof(OrigamiSettings.LastDatabaseMigration).ToLower());

            var rowsAffected = db.Database.ExecuteSqlRaw("UPDATE dbo.oi_Settings SET VALUE = @v WHERE [Name] = @n;", v, n);
            if (rowsAffected == 0)
            {
                db.Database.ExecuteSqlRaw("INSERT INTO dbo.oi_Settings (Id, Value, Name) VALUES (NEWID(), @v, @n);", v, n);
            }

            tr.Complete();
        }
    }
}
