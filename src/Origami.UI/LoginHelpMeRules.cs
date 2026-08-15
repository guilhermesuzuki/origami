using FluentValidation.Internal;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Origami.UI
{
    public class LoginHelpMeRules(
        IAppFacade AppFacade, 
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

        public Stack<ILoginHelpMeRules.Steps> State { get; } = new();

        public bool ShouldDisableMasterPasswordVerification => this.OneTimeMasterPasswordForVerification.Trim().Length < 10;

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

            return Task.CompletedTask;
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
                    this.State.Push(ILoginHelpMeRules.Steps.Step2_CreateNewAdminUser);
                    this.CurrentStepChanged?.Invoke(this, EventArgs.Empty);
                    this.RefreshUI?.Invoke(this, EventArgs.Empty);
                    return;
                }
                if (step == ILoginHelpMeRules.Steps.Step2_CreateNewAdminUser)
                {
                    await this.CreateNewAdminUser();
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

        public Task ValidateMasterPassword()
        {
            var sha256hash = this.OneTimeMasterPasswordForVerification.SHA256Hash();

            if (AppFacade.OneTimeMasterPasswordInSHA256 != sha256hash)
            {
                throw new Exception(Text.Original("Master password doesn't match the system"));
            }

            return Task.CompletedTask;
        }

        protected ILoginHelpMeRules.Steps GetCurrentStep()
        {
            if (State.Count == 0) return ILoginHelpMeRules.Steps.Step1_ValidateMasterPassword;
            return State.Peek();
        }
    }
}
