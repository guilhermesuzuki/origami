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
    public class UserTests : CustomClassFixture
    {
        public UserTests(CustomWebApplicationFactory factory) : base(factory)
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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

            var newPassword = "@" 
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4) 
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4) 
                + "#";

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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ChangePassword_WhenNewPasswordIs1234_ShouldFail()
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

            var newPassword = "1234";
            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), password, newPassword, newPassword);
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeFalse();
            resultNewPassword.Messages.ShouldNotBeNull();
            resultNewPassword.Messages.Count.ShouldBe(3);
            resultNewPassword.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[0].Message.ShouldBe("Password too short");
            resultNewPassword.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[1].Message.ShouldBe("Character was not found in password");
            resultNewPassword.Messages[2].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[2].Message.ShouldBe("Special character was not found in password");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(password.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeTrue();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(password.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ChangePassword_WhenNewPasswordIsEmpty_ShouldFail()
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

            var newPassword = "";
            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), password, newPassword, newPassword);
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeFalse();
            resultNewPassword.Messages.ShouldNotBeNull();
            resultNewPassword.Messages.Count.ShouldBe(1);
            resultNewPassword.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[0].Message.ShouldBe("Password is empty");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(password.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeTrue();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(password.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ChangePassword_WhenNewPasswordIsWeak_ShouldFail()
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

            var newPassword = "weak";
            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), password, newPassword, newPassword);
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeFalse();
            resultNewPassword.Messages.ShouldNotBeNull();
            resultNewPassword.Messages.Count.ShouldBe(3);
            resultNewPassword.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[0].Message.ShouldBe("Password too short");
            resultNewPassword.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[1].Message.ShouldBe("Number was not found in password");
            resultNewPassword.Messages[2].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[2].Message.ShouldBe("Special character was not found in password");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(password.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeTrue();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(password.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ChangePassword_WhenNewPasswordsDifferFromEachOther_ShouldFail()
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

            var newPassword = "@"
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4)
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4)
                + "#";

            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), password, newPassword + "1", newPassword + "2");
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeFalse();
            resultNewPassword.Messages.ShouldNotBeNull();
            resultNewPassword.Messages.Count.ShouldBe(1);
            resultNewPassword.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[0].Message.ShouldBe("New passwords do NOT match, they differ from each other");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(password.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeTrue();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(password.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ChangePassword_WhenNewPasswordsEqualOldPassword_ShouldFail()
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

            var newPassword = password;
            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), password, newPassword, newPassword);
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeFalse();
            resultNewPassword.Messages.ShouldNotBeNull();
            resultNewPassword.Messages.Count.ShouldBe(1);
            resultNewPassword.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[0].Message.ShouldBe("You did NOT change passwords, current and new are the same");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(password.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeTrue();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(password.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ChangePassword_WhenOldPasswordIsWrong_ShouldFail()
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

            var newPassword = "@"
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4)
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4)
                + "#";

            var resultNewPassword = superRepository.Users.ChangePassword(AnotherTestUser.GetContext(TestUser), "wrong-password", newPassword, newPassword);
            resultNewPassword.ShouldNotBeNull();
            resultNewPassword.Ok.ShouldBeFalse();
            resultNewPassword.Messages.ShouldNotBeNull();
            resultNewPassword.Messages.Count.ShouldBe(1);
            resultNewPassword.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultNewPassword.Messages[0].Message.ShouldBe("Username and current password do NOT exist in the database");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.Password.ShouldBe(password.SHA256Hash());
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.MustChangePassword.ShouldBeTrue();
            dbUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.Password.ShouldBe(password.SHA256Hash());
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ForgotOwnPassword_WhenEverythingIsOkay_ShouldPersistRecord()
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultNewPassword = superRepository.Users.ForgotOwnPassword(AnotherTestUser.GetContext(TestUser), true);
            var newPassword = resultNewPassword.Entity!;
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
            dbUser.MustChangePassword.ShouldBeTrue();
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
            cacheUser.MustChangePassword.ShouldBeTrue();
            cacheUser.MustChangePassword.ShouldBe(AnotherTestUser.MustChangePassword);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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
        public void Reset2FA_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            AnotherTestUser.GenerateRandomTOTPSecret();

            var codes = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                var code = Nanoid.Generate(alphabet: Nanoid.Alphabets.LettersAndDigits, size: 6);
                codes.Add(code.SHA256Hash());
            }

            AnotherTestUser.TOTPRecoveryCodes = string.Join(',', codes);

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
            dbUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            dbUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            cacheUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultReset = superRepository.Users.Reset2FA(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultReset.ShouldNotBeNull();
            resultReset.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            dbUser.TOTPSecret.ShouldBeEmpty();
            dbUser.TOTPRecoveryCodes.ShouldBeEmpty();

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
        }

        [Fact]
        public void ResetPassword_WhenUserPossessesKey_ShouldPersistRecord()
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
            dbUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            dbUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            cacheUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultReset = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultReset.ShouldNotBeNull();
            resultReset.Ok.ShouldBeTrue();
            resultReset.Entity.ShouldNotBeNullOrEmpty();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var dbPasswordResets = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResets.Count.ShouldBe(1);
            dbPasswordResets[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResets[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResets = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResets.Count.ShouldBe(1);
            cachePasswordResets[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResets[0].IsDeleted.ShouldBeFalse();

            var newPassword = "@"
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4)
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4)
                + "#";

            var newRole = new OrigamiRole
            {
                Name = "New Role " + Nanoid.Generate(alphabet: Nanoid.Alphabets.LettersAndDigits, size: 6),
                ResetOwnPassword = true,
            };

            scope.CreateTestRole(newRole);
            scope.CreateTestUser(AnotherTestUser, newRole);

            var resultResetAgain = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(AnotherTestUser), dbPasswordResets[0].Key, newPassword, newPassword, checkPermission: true);
            resultResetAgain.ShouldNotBeNull();
            resultResetAgain.Ok.ShouldBeTrue();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            var dbPasswordResetsAgain = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResetsAgain.Count.ShouldBe(1);
            dbPasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResetsAgain[0].IsDeleted.ShouldBeTrue();

            var cachePasswordResetsAgain = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResetsAgain.Count.ShouldBe(1);
            cachePasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResetsAgain[0].IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void ResetPassword_WhenUserPossessesKeyButPasswordsAreWeak_ShouldFail()
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
            dbUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            dbUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            cacheUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultReset = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultReset.ShouldNotBeNull();
            resultReset.Ok.ShouldBeTrue();
            resultReset.Entity.ShouldNotBeNullOrEmpty();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var dbPasswordResets = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResets.Count.ShouldBe(1);
            dbPasswordResets[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResets[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResets = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResets.Count.ShouldBe(1);
            cachePasswordResets[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResets[0].IsDeleted.ShouldBeFalse();

            var newPassword = "weak";

            var newRole = new OrigamiRole
            {
                Name = "New Role " + Nanoid.Generate(alphabet: Nanoid.Alphabets.LettersAndDigits, size: 6),
                ResetOwnPassword = true,
            };

            scope.CreateTestRole(newRole);
            scope.CreateTestUser(AnotherTestUser, newRole);

            var resultResetAgain = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(AnotherTestUser), dbPasswordResets[0].Key, newPassword, newPassword, checkPermission: true);
            resultResetAgain.ShouldNotBeNull();
            resultResetAgain.Ok.ShouldBeFalse();
            resultResetAgain.Messages.ShouldNotBeNull();
            resultResetAgain.Messages.Count.ShouldBe(5);
            resultResetAgain.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[0].Message.ShouldBe("Password too short");
            resultResetAgain.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[1].Message.ShouldBe("Number was not found in password");
            resultResetAgain.Messages[2].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[2].Message.ShouldBe("Special character was not found in password");
            resultResetAgain.Messages[3].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[3].Message.ShouldBe("Failed to reset password");
            resultResetAgain.Messages[4].MessageType.ShouldBe(ResultMessage.MessageTypes.Simple);
            resultResetAgain.Messages[4].Message.ShouldBe("Please, try again later");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            var dbPasswordResetsAgain = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResetsAgain.Count.ShouldBe(1);
            dbPasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResetsAgain[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResetsAgain = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResetsAgain.Count.ShouldBe(1);
            cachePasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResetsAgain[0].IsDeleted.ShouldBeFalse();
        }

        [Fact]
        public void ResetPassword_WhenUserPossessesKeyButPasswordsDiffer_ShouldFail()
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
            dbUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            dbUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            cacheUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultReset = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultReset.ShouldNotBeNull();
            resultReset.Ok.ShouldBeTrue();
            resultReset.Entity.ShouldNotBeNullOrEmpty();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var dbPasswordResets = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResets.Count.ShouldBe(1);
            dbPasswordResets[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResets[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResets = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResets.Count.ShouldBe(1);
            cachePasswordResets[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResets[0].IsDeleted.ShouldBeFalse();

            var newPassword = "@"
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4)
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4)
                + "#";

            var newRole = new OrigamiRole
            {
                Name = "New Role " + Nanoid.Generate(alphabet: Nanoid.Alphabets.LettersAndDigits, size: 6),
                ResetOwnPassword = true,
            };

            scope.CreateTestRole(newRole);
            scope.CreateTestUser(AnotherTestUser, newRole);

            var resultResetAgain = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(AnotherTestUser), dbPasswordResets[0].Key, newPassword + "1", newPassword + "2", checkPermission: true);
            resultResetAgain.ShouldNotBeNull();
            resultResetAgain.Ok.ShouldBeFalse();
            resultResetAgain.Messages.ShouldNotBeNull();
            resultResetAgain.Messages.Count.ShouldBe(4);
            resultResetAgain.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Success);
            resultResetAgain.Messages[0].Message.ShouldBe("Password is strong");
            resultResetAgain.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[1].Message.ShouldBe("New passwords do NOT match, they differ from each other");
            resultResetAgain.Messages[2].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[2].Message.ShouldBe("Failed to reset password");
            resultResetAgain.Messages[3].MessageType.ShouldBe(ResultMessage.MessageTypes.Simple);
            resultResetAgain.Messages[3].Message.ShouldBe("Please, try again later");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            var dbPasswordResetsAgain = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResetsAgain.Count.ShouldBe(1);
            dbPasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResetsAgain[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResetsAgain = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResetsAgain.Count.ShouldBe(1);
            cachePasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResetsAgain[0].IsDeleted.ShouldBeFalse();
        }

        [Fact]
        public void ResetPassword_WhenUserPossessesTheWrongKey_ShouldFail()
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
            dbUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            dbUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            cacheUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultReset = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultReset.ShouldNotBeNull();
            resultReset.Ok.ShouldBeTrue();
            resultReset.Entity.ShouldNotBeNullOrEmpty();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var dbPasswordResets = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResets.Count.ShouldBe(1);
            dbPasswordResets[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResets[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResets = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResets.Count.ShouldBe(1);
            cachePasswordResets[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResets[0].IsDeleted.ShouldBeFalse();

            var newPassword = "@"
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4)
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4)
                + "#";

            var newRole = new OrigamiRole
            {
                Name = "New Role " + Nanoid.Generate(alphabet: Nanoid.Alphabets.LettersAndDigits, size: 6),
                ResetOwnPassword = true,
            };

            scope.CreateTestRole(newRole);
            scope.CreateTestUser(AnotherTestUser, newRole);

            var resultResetAgain = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(AnotherTestUser), dbPasswordResets[0].Key + "123", newPassword, newPassword, checkPermission: true);
            resultResetAgain.ShouldNotBeNull();
            resultResetAgain.Ok.ShouldBeFalse();
            resultResetAgain.Messages.ShouldNotBeNull();
            resultResetAgain.Messages.Count.ShouldBe(4);
            resultResetAgain.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Success);
            resultResetAgain.Messages[0].Message.ShouldBe("Password is strong");
            resultResetAgain.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[1].Message.ShouldBe("Password reset key is invalid or has already been used");
            resultResetAgain.Messages[2].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultResetAgain.Messages[2].Message.ShouldBe("Failed to reset password");
            resultResetAgain.Messages[3].MessageType.ShouldBe(ResultMessage.MessageTypes.Simple);
            resultResetAgain.Messages[3].Message.ShouldBe("Please, try again later");

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            var dbPasswordResetsAgain = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResetsAgain.Count.ShouldBe(1);
            dbPasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResetsAgain[0].IsDeleted.ShouldBeFalse();

            var cachePasswordResetsAgain = superRepository.UserPasswordResets.ReadFromCache().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            cachePasswordResetsAgain.Count.ShouldBe(1);
            cachePasswordResetsAgain[0].Key.ShouldBe(resultReset.Entity);
            cachePasswordResetsAgain[0].IsDeleted.ShouldBeFalse();
        }

        [Fact]
        public void ResetPassword_WhenUserIsNotLoggedIn_ShouldPersistRecord()
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
            dbUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            dbUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            var cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBe(AnotherTestUser.TOTPSecret);
            cacheUser.TOTPRecoveryCodes.ShouldBe(AnotherTestUser.TOTPRecoveryCodes);

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

            var resultReset = superRepository.Users.ResetPassword(AnotherTestUser.GetContext(TestUser), checkPermission: true);
            resultReset.ShouldNotBeNull();
            resultReset.Ok.ShouldBeTrue();
            resultReset.Entity.ShouldNotBeNullOrEmpty();

            dbUser = superRepository.Users.ReadFromDatabase(AnotherTestUser);
            dbUser.ShouldNotBeNull();
            dbUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            dbUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            dbUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            dbUser.LastName.ShouldBe(AnotherTestUser.LastName);
            dbUser.Username.ShouldBe(AnotherTestUser.Username);
            dbUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            dbUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);

            cacheUser = superRepository.Users.ReadFromCache().Id(AnotherTestUser.Id);
            cacheUser.ShouldNotBeNull();
            cacheUser.DateCreated.ShouldBe(AnotherTestUser.DateCreated);
            cacheUser.DisplayName.ShouldBe(AnotherTestUser.DisplayName);
            cacheUser.FirstName.ShouldBe(AnotherTestUser.FirstName);
            cacheUser.LastName.ShouldBe(AnotherTestUser.LastName);
            cacheUser.Username.ShouldBe(AnotherTestUser.Username);
            cacheUser.NanoId.ShouldBe(AnotherTestUser.NanoId);
            cacheUser.IsDeleted.ShouldBe(AnotherTestUser.IsDeleted);
            cacheUser.TOTPSecret.ShouldBeEmpty();
            cacheUser.TOTPRecoveryCodes.ShouldBeEmpty();

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var dbPasswordResets = db.UserPasswordResets.AsNoTracking().Where(x => x.UserId == AnotherTestUser.Id).ToList();
            dbPasswordResets.Count.ShouldBe(1);
            dbPasswordResets[0].Key.ShouldBe(resultReset.Entity);
            dbPasswordResets[0].IsDeleted.ShouldBeFalse();
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);
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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(AnotherTestUser.Version);

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

            cacheUser.Version.ShouldBe(dbUser.Version);
            cacheUser.Version.ShouldBe(updateUser.Version);
        }
    }
}
