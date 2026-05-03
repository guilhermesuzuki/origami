using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NanoidDotNet;
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
        public void Block_WhenEntityIsAlreadyBlocked_ShouldPersistRecord()
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
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(false);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(false);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var resultBlock = superRepository.Users.Block(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultBlock.ShouldNotBeNull();
            resultBlock.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(true);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            dbUser.DateBlocked.ShouldNotBeNull();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(true);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            cacheUser.DateBlocked.ShouldNotBeNull();

            var resultBlockAgain = superRepository.Users.Block(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultBlockAgain.ShouldNotBeNull();
            resultBlockAgain.Ok.ShouldBeFalse();
            resultBlockAgain.Messages.ShouldNotBeNull();
            resultBlockAgain.Messages.Count.ShouldBe(1);
            resultBlockAgain.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultBlockAgain.Messages[0].Message.ShouldBe("User is already blocked");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(true);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            dbUser.DateBlocked.ShouldNotBeNull();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(true);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            cacheUser.DateBlocked.ShouldNotBeNull();
        }

        [Fact]
        public void Block_WhenEntityIsValid_ShouldPersistRecord()
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
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(false);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(false);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var resultBlock = superRepository.Users.Block(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultBlock.ShouldNotBeNull();
            resultBlock.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(true);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            dbUser.DateBlocked.ShouldNotBeNull();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(true);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            cacheUser.DateBlocked.ShouldNotBeNull();
        }

        [Fact]
        public void ChangePassword_WhenEverythingIsOkay_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var result = superRepository.Users.SmartSave(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();
            result.Messages.Count.ShouldBe(2);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Info);
            result.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Password);

            var password = result.Messages[1].Message;

            var userPassword = superRepository.Users.LookupUserInDatabase(AnotherTestUser.Username, password);
            userPassword.ShouldNotBeNull();
            userPassword.Username.ShouldBe(AnotherTestUser.Username);
            userPassword.Password.ShouldBe(password.SHA256Hash());

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

            var newPassword = "@" + Nanoid.Generate(size: 8) + "#";
            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), password, newPassword, newPassword);
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(newPassword.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeFalse();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(newPassword.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeFalse();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);
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
            result.Messages.Count.ShouldBe(2);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Info);
            result.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Password);

            var password = result.Messages[1].Message;

            var userPassword = superRepository.Users.LookupUserInDatabase(AnotherTestUser.Username, password);
            userPassword.ShouldNotBeNull();
            userPassword.Username.ShouldBe(AnotherTestUser.Username);
            userPassword.Password.ShouldBe(password.SHA256Hash());

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
        public void Unblock_WhenEntityIsBlocked_ShouldPersistRecord()
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
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(false);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(false);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var resultBlock = superRepository.Users.Block(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultBlock.ShouldNotBeNull();
            resultBlock.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(true);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            dbUser.DateBlocked.ShouldNotBeNull();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(true);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            cacheUser.DateBlocked.ShouldNotBeNull();

            var resultUnblock = superRepository.Users.Unblock(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultUnblock.ShouldNotBeNull();
            resultUnblock.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(false);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            dbUser.DateUnblocked.ShouldNotBeNull();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(false);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            cacheUser.DateUnblocked.ShouldNotBeNull();
        }

        [Fact]
        public void Unblock_WhenEntityIsAlreadyUnblocked_ShouldPersistRecord()
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
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(false);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(false);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);

            var resultUnblock = superRepository.Users.Unblock(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultUnblock.ShouldNotBeNull();
            resultUnblock.Ok.ShouldBeFalse();
            resultUnblock.Messages.ShouldNotBeNull();
            resultUnblock.Messages.Count.ShouldBe(1);
            resultUnblock.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultUnblock.Messages[0].Message.ShouldBe("User is already unblocked");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(false);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.IsBlocked.ShouldBe(false);
            dbUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            dbUser.DateUnblocked.ShouldBeNull();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsDeleted.ShouldBe(false);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.IsBlocked.ShouldBe(false);
            cacheUser.IsBlocked.ShouldBe(AnotherTestUser.IsBlocked);
            cacheUser.DateUnblocked.ShouldBeNull();
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
