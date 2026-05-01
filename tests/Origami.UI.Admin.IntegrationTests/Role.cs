using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class Role : CustomClassFixture
    {
        public Role(CustomWebApplicationFactory factory) : base(factory)
        {

        }

        [Fact]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var anotherRole = new OrigamiRole
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Name = "Another test role",
                NanoId = Guid.NewGuid().ToString().Substring(0, 8),
            };

            var result = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var resultDelete = superRepository.Roles.SmartDelete(anotherRole.GetContext(TestUser), checkPermission: true);
            resultDelete.ShouldNotBeNull();
            resultDelete.Ok.ShouldBeTrue();

            var dbRole = superRepository.Roles.ReadFromDatabase(anotherRole);
            dbRole.ShouldNotBeNull();
            dbRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            dbRole.Name.ShouldBe(anotherRole.Name);
            dbRole.NanoId.ShouldBe(anotherRole.NanoId);
            dbRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);

            var cacheRole = superRepository.Roles.ReadFromCache().Id(anotherRole.Id);
            cacheRole.ShouldNotBeNull();
            cacheRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            cacheRole.Name.ShouldBe(anotherRole.Name);
            cacheRole.NanoId.ShouldBe(anotherRole.NanoId);
            cacheRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var anotherRole = new OrigamiRole 
            { 
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Name = "Another test role",
                NanoId = Guid.NewGuid().ToString().Substring(0, 8),
                CreateNewBlogs = true,
                DeleteBlogs = true,
                EditBlogs = true,
                PurgeBlogs = true, 
            };

            var result = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var dbRole = superRepository.Roles.ReadFromDatabase(anotherRole);
            dbRole.ShouldNotBeNull();
            dbRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            dbRole.Name.ShouldBe(anotherRole.Name);
            dbRole.NanoId.ShouldBe(anotherRole.NanoId);
            dbRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);

            var cacheRole = superRepository.Roles.ReadFromCache().Id(anotherRole.Id);
            cacheRole.ShouldNotBeNull();
            cacheRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            cacheRole.Name.ShouldBe(anotherRole.Name);
            cacheRole.NanoId.ShouldBe(anotherRole.NanoId);
            cacheRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var query = from a in db.RightRoles
                        join b in db.Rights on a.RightId equals b.Id
                        where a.RoleId == anotherRole.Id
                        orderby b.Name
                        select b;

            var rights = query.ToList();
            rights.Count.ShouldBe(4);
            rights[0].ShouldNotBeNull();
            rights[1].ShouldNotBeNull();
            rights[2].ShouldNotBeNull();
            rights[3].ShouldNotBeNull();
            rights[0].Name.ShouldBe(nameof(OrigamiRole.CreateNewBlogs));
            rights[1].Name.ShouldBe(nameof(OrigamiRole.DeleteBlogs));
            rights[2].Name.ShouldBe(nameof(OrigamiRole.EditBlogs));
            rights[3].Name.ShouldBe(nameof(OrigamiRole.PurgeBlogs));
        }

        [Fact]
        public void Insert_WhenNameIsTooLarge_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var anotherRole = new OrigamiRole
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Name = "Another test role " + new string('a', 256),
                NanoId = Guid.NewGuid().ToString().Substring(0, 8),
            };

            var result = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(1);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Name cannot exceed 255 characters");

            var role = superRepository.Roles.ReadFromDatabase(anotherRole);
            role.ShouldBeNull();

            var cacheRole = superRepository.Roles.ReadFromCache().Id(anotherRole.Id);
            cacheRole.ShouldBeNull();
        }

        [Fact]
        public void Purge_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var anotherRole = new OrigamiRole
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Name = "Another test role",
                NanoId = Guid.NewGuid().ToString().Substring(0, 8),
            };

            var result = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var resultPurge = superRepository.Roles.SmartPurge(anotherRole.GetContext(TestUser), checkPermission: true);
            resultPurge.ShouldNotBeNull();
            resultPurge.Ok.ShouldBeTrue();

            var dbRole = superRepository.Roles.ReadFromDatabase(anotherRole);
            dbRole.ShouldBeNull();

            var cacheRole = superRepository.Roles.ReadFromCache().Id(anotherRole.Id);
            cacheRole.ShouldBeNull();

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var query = from a in db.RightRoles
                        join b in db.Rights on a.RightId equals b.Id
                        where a.RoleId == anotherRole.Id
                        orderby b.Name
                        select b;

            var rights = query.ToList();
            rights.ShouldNotBeNull();
            rights.Count.ShouldBe(0);
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var anotherRole = new OrigamiRole
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Name = "Another test role",
                NanoId = Guid.NewGuid().ToString().Substring(0, 8),
            };

            var result = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            anotherRole.Name = "Another updated test role";

            var resultUpdate = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);
            resultUpdate.ShouldNotBeNull();
            resultUpdate.Ok.ShouldBeTrue();

            var dbRole = superRepository.Roles.ReadFromDatabase(anotherRole);
            dbRole.ShouldNotBeNull();
            dbRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            dbRole.Name.ShouldBe(anotherRole.Name);
            dbRole.NanoId.ShouldBe(anotherRole.NanoId);
            dbRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);

            var cacheRole = superRepository.Roles.ReadFromCache().Id(anotherRole.Id);
            cacheRole.ShouldNotBeNull();
            cacheRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            cacheRole.Name.ShouldBe(anotherRole.Name);
            cacheRole.NanoId.ShouldBe(anotherRole.NanoId);
            cacheRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);
        }

        [Fact]
        public void Update_WhenNameIsTooLarge_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var anotherRole = new OrigamiRole
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Name = "Another test role",
                NanoId = Guid.NewGuid().ToString().Substring(0, 8),
            };

            var result = superRepository.Roles.SmartSave(anotherRole.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var anotherRoleUpdate = anotherRole.Clone();
            anotherRoleUpdate.Name = "Another updated test role " + new string('a', 255);

            var resultUpdate = superRepository.Roles.SmartSave(anotherRoleUpdate.GetContext(TestUser), checkPermission: true);
            resultUpdate.ShouldNotBeNull();
            resultUpdate.Ok.ShouldBeFalse();
            resultUpdate.Messages.ShouldNotBeNull();
            resultUpdate.Messages.Count.ShouldBe(1);
            resultUpdate.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultUpdate.Messages[0].Message.ShouldBe("Name cannot exceed 255 characters");

            var dbRole = superRepository.Roles.ReadFromDatabase(anotherRole);
            dbRole.ShouldNotBeNull();
            dbRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            dbRole.Name.ShouldBe(anotherRole.Name);
            dbRole.NanoId.ShouldBe(anotherRole.NanoId);
            dbRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);

            var cacheRole = superRepository.Roles.ReadFromCache().Id(anotherRole.Id);
            cacheRole.ShouldNotBeNull();
            cacheRole.DateCreated.ShouldBe(anotherRole.DateCreated);
            cacheRole.Name.ShouldBe(anotherRole.Name);
            cacheRole.NanoId.ShouldBe(anotherRole.NanoId);
            cacheRole.IsDeleted.ShouldBe(anotherRole.IsDeleted);
        }
    }
}
