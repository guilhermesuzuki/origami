using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface ILoginRules
    {
        event EventHandler CurrentStepChanged;
        event EventHandler RefreshUI;
        event EventHandler WelcomeToTheApplication;

        public enum Steps
        {
            Step1_ValidateCredentials = 1,
            Step2_MustChangePassword = 2,
            Step3_MustEnable2MFA = 3,
            Step4_Validate2FA = 4,
            Step5_WelcomeToTheApplication = 5,
        }

        string NewPassword1 { get; set; }
        string NewPassword2 { get; set; }
        string Password { get; set; }
        
        bool ShouldDisable2FAEnablement { get; }
        bool ShouldDisable2FAValidation { get; }
        bool ShouldDisableLogin { get; }
        bool ShouldDisablePasswordChange { get; }

        Stack<Steps> State { get; }
        
        string TOTPCodeForEnablement { get; set; }
        string TOTPCodeForValidation { get; set; }
        string[] TOTPRecoveryCodes { get; set; }

        OrigamiUser User { get; set; }

        string Username { get; set; }

        Task ChangePasswordAsync();

        void Clear2FA();
        void ClearChangePassword();
        void ClearCredentials();
        void ClearEnable2FA();

        Task Enable2FAAsync();

        Steps GetCurrentStep();

        Task GoBackAsync();
        Task GoNextAsync();
        Task LoginAsync();
        
        void Regenerate2FARecoveryCodes();
        void Regenerate2FASecret();

        Task ResetAsync();
        Task Validate2FAAsync();
    }
}
