using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class User : CustomClassFixture
    {
        public User(CustomWebApplicationFactory factory) : base(factory)
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

            var result = superRepository.Users.SmartSave(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var resultDelete = superRepository.Users.SmartDelete(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultDelete.ShouldNotBeNull();
            resultDelete.Ok.ShouldBeTrue();

            var dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(true);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var result = superRepository.Users.SmartSave(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
        }

        [Fact]
        public void Purge_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var result = superRepository.Users.SmartSave(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var resultPurge = superRepository.Users.SmartPurge(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultPurge.ShouldNotBeNull();
            resultPurge.Ok.ShouldBeTrue();

            var dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldBeNull();

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldBeNull();
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var result = superRepository.Users.SmartSave(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            var updateUser = AnotherTestUser.Clone();
            updateUser.DisplayName = "Updated display name";

            var resultUpdate = superRepository.Users.SmartSave(updateUser.GetContext(TestUser), checkPermission: true);
            resultUpdate.ShouldNotBeNull();
            resultUpdate.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(updateUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(updateUser.DateCreated);
            dbUser.DisplayName.ShouldBe(updateUser.DisplayName);
            dbUser.FirstName.ShouldBe(updateUser.FirstName);
            dbUser.LastName.ShouldBe(updateUser.LastName);
            dbUser.Username.ShouldBe(updateUser.Username);
            dbUser.NanoId.ShouldBe(updateUser.NanoId);
            dbUser.IsDeleted.ShouldBe(updateUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(updateUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(updateUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(updateUser.DisplayName);
            cacheUser.FirstName.ShouldBe(updateUser.FirstName);
            cacheUser.LastName.ShouldBe(updateUser.LastName);
            cacheUser.Username.ShouldBe(updateUser.Username);
            cacheUser.NanoId.ShouldBe(updateUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(updateUser.IsDeleted);
        }
    }
}
