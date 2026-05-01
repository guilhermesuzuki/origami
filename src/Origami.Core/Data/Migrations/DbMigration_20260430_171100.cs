using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Transactions;

namespace Origami.Core.Data.Migrations
{
    public class DbMigration_20260430_171100 : DbMigration
    {
        public DbMigration_20260430_171100(ISuperRepository superRepository) : base(superRepository)
        {

        }

        public override DateTime Key => new DateTime(2026, 4, 30, 17, 11, 0);

        public override bool HasBeenApplied()
        {
            using var db = _superRepository.DbContextFactory.CreateDbContext();
            using var connection = new SqlConnection(db.Database.GetConnectionString());

            IEnumerable<string> commands = [
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'oi_Blogs' AND COLUMN_NAME = 'Slug' AND DATA_TYPE = 'nvarchar' AND CHARACTER_MAXIMUM_LENGTH = 255 AND IS_NULLABLE = 'NO';",
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'oi_Categories' AND COLUMN_NAME = 'Slug' AND DATA_TYPE = 'nvarchar' AND CHARACTER_MAXIMUM_LENGTH = 50 AND IS_NULLABLE = 'NO';",
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'oi_ContentTags' AND COLUMN_NAME = 'Slug' AND DATA_TYPE = 'nvarchar' AND CHARACTER_MAXIMUM_LENGTH = 128 AND IS_NULLABLE = 'NO';",
                ];

            try
            {
                connection.Open();

                foreach (var command in commands)
                {
                    var sqlCmd = new SqlCommand(command, connection);
                    var count = (int)sqlCmd.ExecuteScalar();
                    if (count != 1)
                    {
                        return false;
                    }
                }

                var blogs = db.Blogs.Any(x => string.IsNullOrWhiteSpace(x.Slug));
                var categories = db.Categories.Any(x => string.IsNullOrWhiteSpace(x.Slug));
                var contentTags = db.ContentTags.Any(x => string.IsNullOrWhiteSpace(x.Slug));

                if (blogs || categories || contentTags)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                connection.Close();
            }

            return false;
        }

        public override void Migrate()
        {
            using var transaction = new TransactionScope();
            using var db = _superRepository.DbContextFactory.CreateDbContext();

            db.Database.ExecuteSqlRaw(@"
                ALTER TABLE dbo.oi_Blogs ADD Slug NVARCHAR(255) NULL;
                ALTER TABLE dbo.oi_Categories ADD Slug NVARCHAR(50) NULL;
                ALTER TABLE dbo.oi_ContentTags ADD Slug NVARCHAR(128) NULL;
            ");

            var blogs = db.Database.SqlQueryRaw<__Blog>("SELECT Id, Name FROM dbo.oi_Blogs").ToList();
            var categories = db.Database.SqlQueryRaw<__Category>("SELECT Id, Name FROM dbo.oi_Categories").ToList();
            var tags = db.Database.SqlQueryRaw<__Tag>("SELECT Id, Tag FROM dbo.oi_ContentTags").ToList();

            foreach (var blog in blogs)
            {
                var slug = blog.Name.GetSlug();
                var pId = new SqlParameter("@Id", blog.Id);
                var pSlug = new SqlParameter("@Slug", slug);
                db.Database.ExecuteSqlRaw("UPDATE dbo.oi_Blogs SET Slug = @Slug WHERE Id = @Id", [pSlug, pId]);
            }

            foreach (var category in categories)
            {
                var slug = category.Name.GetSlug();
                var pId = new SqlParameter("@Id", category.Id);
                var pSlug = new SqlParameter("@Slug", slug);
                db.Database.ExecuteSqlRaw("UPDATE dbo.oi_Categories SET Slug = @Slug WHERE Id = @Id", [pSlug, pId]);
            }

            foreach (var tag in tags)
            {
                var slug = tag.Tag.GetSlug();
                var pId = new SqlParameter("@Id", tag.Id);
                var pSlug = new SqlParameter("@Slug", slug);
                db.Database.ExecuteSqlRaw("UPDATE dbo.oi_ContentTags SET Slug = @Slug WHERE Id = @Id", [pSlug, pId]);
            }

            db.Database.ExecuteSqlRaw(@"
                ALTER TABLE dbo.oi_Blogs ALTER COLUMN Slug NVARCHAR(255) NOT NULL;
                ALTER TABLE dbo.oi_Categories ALTER COLUMN Slug NVARCHAR(50) NOT NULL;
                ALTER TABLE dbo.oi_ContentTags ALTER COLUMN Slug NVARCHAR(128) NOT NULL;

                CREATE UNIQUE NONCLUSTERED INDEX UX_oi_Blogs_Slug ON dbo.oi_Blogs (Slug);
                CREATE UNIQUE NONCLUSTERED INDEX UX_oi_Categories_Slug ON dbo.oi_Categories (Slug);
                CREATE UNIQUE NONCLUSTERED INDEX UX_oi_ContentTags_Slug ON dbo.oi_ContentTags (ContentId,Slug);
            ");

            transaction.Complete();
        }

        private class __Blog
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class __Category
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class __Tag
        {
            public Guid Id { get; set; }
            public string Tag { get; set; } = string.Empty;
        }
    }
}
