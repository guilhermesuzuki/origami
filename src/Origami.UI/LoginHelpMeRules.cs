using Bogus;
using MudBlazor;
using NanoidDotNet;
using Octokit;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Diagnostics;
using System.Transactions;

namespace Origami.UI
{
    public class LoginHelpMeRules(
        IAppFacade AppFacade,
        IDialogService DialogService,
        IUserFacade UserFacade,
        IUserRepository UserRepository,
        IUserRoleRepository UserRoleRepository,
        IUserBlogRepository UserBlogRepository,
        Text Text
        ) : ILoginHelpMeRules
    {
        public event EventHandler CurrentStepChanged = null!;

        public event EventHandler GoToLoginPage = null!;

        public event EventHandler RefreshUI = null!;

        public OrigamiUser Entity { get; set; } = GetCleanUser();

        public string OneTimeMasterPasswordForVerification { get; set; } = string.Empty;

        public bool ShouldDisableMasterPasswordVerification => this.OneTimeMasterPasswordForVerification.Trim().Length < 10;

        public Stack<ILoginHelpMeRules.Steps> State { get; } = new();

        public static OrigamiUser GetCleanUser()
        {
            var faker = new Faker<OrigamiUser>()
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.EmailAddress, f => f.Internet.Email())
                .RuleFor(x => x.DisplayName, (f, u) => $"{u.FirstName}.{u.LastName}".ToLower());

            var user = faker.Generate();

            user.Username = "user_" + Nanoid.Generate(Nanoid.Alphabets.LowercaseLetters, size: 5);
            user.EmailAddress = $"fake_user.{Nanoid.Generate(Nanoid.Alphabets.Digits, size: 5)}@fake-email.com";
            user.GenerateNewPasswordForNewUsers();

            return user;
        }

        public Task CreateNewUser()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var ctx = new DataOperationContext<OrigamiUser>(OrigamiUser.AnonymousUser, this.Entity);
            var hub = UserRepository.SmartSave(ctx, false);

            if (hub.Ok == false)
            {
                throw new Exception(hub.GetMessages());
            }

            if (hub.Ok)
            {
                var cts = this.Entity.UserRoles.GetContexts(ctx);
                cts.Each(x =>
                {
                    x.Entity.UserId = ctx.Entity.Id;
                    UserRoleRepository.SmartSave(x, false).Push(hub);
                });
            }

            if (hub.Ok)
            {
                var cts = this.Entity.UserBlogs.GetContexts(ctx);
                cts.Each(x => 
                {
                    x.Entity.UserId = ctx.Entity.Id;
                    UserBlogRepository.SmartSave(x, false).Push(hub);
                });
            }

            transaction.Complete();
            return Task.CompletedTask;
        }

        public ILoginHelpMeRules.Steps GetCurrentStep()
        {
            if (State.Count == 0) return ILoginHelpMeRules.Steps.Step1_ValidateMasterPassword;
            return State.Peek();
        }

        public async Task GoBackAsync()
        {
            var step = this.GetCurrentStep();

            if (step == ILoginHelpMeRules.Steps.Step1_ValidateMasterPassword)
            {
                await this.RedirectUserToLoginPage();
                return;
            }

            if (step == ILoginHelpMeRules.Steps.Step2_CreateNewUser)
            {
                this.State.Pop();
                this.CurrentStepChanged?.Invoke(this, EventArgs.Empty);
                this.RefreshUI?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        public async Task GoNextAsync()
        {
            var step = this.GetCurrentStep();

            try
            {
                if (step == ILoginHelpMeRules.Steps.Step1_ValidateMasterPassword)
                {
                    await this.ValidateMasterPassword();

                    var yes = await DialogService.ShowMessageBoxAsync(
                        Text.Upper("One-time master password"),
                        Text.Lower("Are you ready to use the one-time master password and create a new admin user?"),
                        Text.Lower("Yes"),
                        Text.Lower("No")
                        );

                    if (yes.GetValueOrDefault() == false)
                    {
                        return;
                    }

                    this.State.Push(ILoginHelpMeRules.Steps.Step2_CreateNewUser);
                    this.CurrentStepChanged?.Invoke(this, EventArgs.Empty);
                    this.RefreshUI?.Invoke(this, EventArgs.Empty);

                    if (Debugger.IsAttached == false)
                    {
                        AppFacade.OneTimeMasterPasswordInSHA256 = string.Empty;
                        UserFacade.Result = new() { Warning = Text.Original("One-time master password used") };
                    }

                    return;
                }
                if (step == ILoginHelpMeRules.Steps.Step2_CreateNewUser)
                {
                    await this.CreateNewUser();
                    this.State.Push(ILoginHelpMeRules.Steps.Step3_GoToLoginPage);
                    this.CurrentStepChanged?.Invoke(this, EventArgs.Empty);
                    this.RefreshUI?.Invoke(this, EventArgs.Empty);
                    await this.RedirectUserToLoginPage();
                    return;
                }
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
            }
        }

        public Task RedirectUserToLoginPage()
        {
            GoToLoginPage?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task ValidateMasterPassword()
        {
            var sha256hash = this.OneTimeMasterPasswordForVerification.SHA256Hash();

            if (AppFacade.OneTimeMasterPasswordInSHA256 != sha256hash)
            {
                await Task.Delay(2000);
                throw new Exception(Text.Original("Master password doesn't match the system"));
            }
        }
    }
}
