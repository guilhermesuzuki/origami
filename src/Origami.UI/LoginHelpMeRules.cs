using MudBlazor;
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
        Text Text
        ) : ILoginHelpMeRules
    {
        public event EventHandler CurrentStepChanged = null!;

        public event EventHandler GoToLoginPage = null!;

        public event EventHandler RefreshUI = null!;

        public OrigamiUser NewAdminUser { get; set; } = new();
        
        public string OneTimeMasterPasswordForVerification { get; set; } = string.Empty;
        
        public IList<OrigamiUserRole> RolesForTheNewAdminUser { get; } = new List<OrigamiUserRole>();

        public bool ShouldDisableMasterPasswordVerification => this.OneTimeMasterPasswordForVerification.Trim().Length < 10;

        public Stack<ILoginHelpMeRules.Steps> State { get; } = new();

        public Task CreateNewAdminUser()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var ctx = new DataOperationContext<OrigamiUser>(OrigamiUser.AnonymousUser, this.NewAdminUser);
            var hub = UserRepository.SmartSave(ctx, false);

            var cts = this.RolesForTheNewAdminUser.GetContexts(ctx);
            cts.Each(x => UserRoleRepository.SmartSave(x, false).Push(hub));

            if (hub.Ok == false)
            {
                throw new Exception(hub.GetMessages());
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

            if (step == ILoginHelpMeRules.Steps.Step2_CreateNewAdminUser)
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
                        Text.Upper("1-time master password"),
                        Text.Lower("Are you ready to use the 1-time master password and create a new admin user?"),
                        Text.Lower("Yes"),
                        Text.Lower("No")
                        );

                    if (yes.GetValueOrDefault() == false)
                    {
                        return;
                    }

                    this.State.Push(ILoginHelpMeRules.Steps.Step2_CreateNewAdminUser);
                    this.CurrentStepChanged?.Invoke(this, EventArgs.Empty);
                    this.RefreshUI?.Invoke(this, EventArgs.Empty);

                    if (Debugger.IsAttached == false)
                    {
                        AppFacade.OneTimeMasterPasswordInSHA256 = string.Empty;
                        UserFacade.Result = new() { Warning = Text.Original("1-time master password used") };
                    }
                    
                    return;
                }
                if (step == ILoginHelpMeRules.Steps.Step2_CreateNewAdminUser)
                {
                    await this.CreateNewAdminUser();

                    var yes = await DialogService.ShowMessageBoxAsync(
                        Text.Upper("Creating a user"),
                        Text.Lower("Are you done with creating a new user? And go straight to the login page"),
                        Text.Lower("Yes"),
                        Text.Lower("No")
                        );

                    if (yes.GetValueOrDefault() == false)
                    {
                        return;
                    }

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

        public OrigamiUser GetCleanUser()
        {
            return new();
        }
    }
}
