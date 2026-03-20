using NanoidDotNet;
using Origami.Core.Models;
using OtpNet;

namespace Origami.Core.Data
{
    public class LoginRepository : ILoginRepository
    {
        protected readonly ISuperRepository _superRepository;
        protected readonly Text _text;
        protected readonly IUserFacade _userFacade;

        public LoginRepository(ISuperRepository superRepository, IUserFacade userFacade, Text text)
        {
            _superRepository = superRepository;
            _userFacade = userFacade;
            _text = text;

            this.TOTPRecoveryCodes = _superRepository.Users.GenerateTOTPRecoveryCodes();

            this.ResetAsync().Wait();
        }

        public event EventHandler CurrentStepChanged = null!;
        public event EventHandler RefreshUI = null!;
        public event EventHandler WelcomeToTheApplication = null!;

        public string NewPassword1 { get; set; } = string.Empty;
        public string NewPassword2 { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public bool ShouldDisable2FAEnablement
        {
            get
            {
                if (TOTPCodeForEnablement.Has() == false) return true;
                if (TOTPCodeForEnablement.Trim().Length != 6) return true;
                return false;
            }
        }

        public bool ShouldDisable2FAValidation
        {
            get
            {
                if (TOTPCodeForValidation.Has() == false) return true;
                if (TOTPCodeForValidation.Trim().Length != 6) return true;
                return false;
            }
        }

        public bool ShouldDisableLogin
        {
            get
            {
                if (Username.Has() == false) return true;
                if (Password.Has() == false) return true;
                return false;
            }
        }

        public bool ShouldDisablePasswordChange
        {
            get
            {
                if (NewPassword1.Has() == false) return true;
                if (NewPassword2.Has() == false) return true;
                return false;
            }
        }

        public Stack<ILoginRepository.Steps> State { get; } = new Stack<ILoginRepository.Steps>();

        public string TOTPCodeForEnablement { get; set; } = string.Empty;

        public string TOTPCodeForValidation { get; set; } = string.Empty;

        public string[] TOTPRecoveryCodes { get; set; } = [];

        public OrigamiUser User { get; set; } = new();

        public string Username { get; set; } = string.Empty;

        public async Task ChangePasswordAsync()
        {
            var hub = this._superRepository.Users.ChangePassword(
                this.User.GetContext(),
                this.Password,
                this.NewPassword1,
                this.NewPassword2);

            try
            {
                if (hub.Ok)
                {
                    await GoNextAsync();
                    return;
                }
            }
            finally
            {
                this._userFacade.Result = hub;
            }

            throw new InvalidOperationException("Failed to change password.");
        }

        public void Clear2FA()
        {
            this.TOTPCodeForValidation = string.Empty;
        }

        public void ClearChangePassword()
        {
            this.NewPassword1 = string.Empty;
            this.NewPassword2 = string.Empty;
        }

        public void ClearCredentials()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
        }

        public void ClearEnable2FA()
        {
            this.TOTPCodeForEnablement = string.Empty;
        }

        public Task Enable2FAAsync()
        {
            var secretBytes = Base32Encoding.ToBytes(this.User.TOTPSecret.ToString());
            var totp = new Totp(secretBytes);
            var valid = totp.VerifyTotp(this.TOTPCodeForEnablement, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (valid)
            {
                var ctx = this.User.GetContext();
                var hub = this._superRepository.Users.SmartSave(ctx, false);
                try
                {
                    if (hub.Ok)
                    {
                        return Task.CompletedTask;
                    }
                }
                finally
                {
                    this._userFacade.Result = hub;
                }
            }

            throw new InvalidOperationException("Failed to enable 2FA.");
        }

        public ILoginRepository.Steps GetCurrentStep()
        {
            if (State.Count == 0) return ILoginRepository.Steps.Step1_ValidateCredentials;
            return State.Peek();
        }

        public Task GoBackAsync()
        {
            this.State.Clear();
            this.State.Push(ILoginRepository.Steps.Step1_ValidateCredentials);
            this.RefreshUI?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task GoNextAsync()
        {
            try
            {
                var step = this.GetCurrentStep();
                if (step == ILoginRepository.Steps.Step1_ValidateCredentials)
                {
                    await this.LoginAsync();

                    if (this.User.MustChangePassword == true)
                    {
                        this._userFacade.Result = new() { Info = _text.Original("You must change your password") };
                        this.State.Push(ILoginRepository.Steps.Step2_MustChangePassword);
                        return;
                    }

                    _2FA();
                }
                else if (step == ILoginRepository.Steps.Step2_MustChangePassword)
                {
                    await this.ChangePasswordAsync();
                    _2FA();
                }
                else if (step == ILoginRepository.Steps.Step3_MustEnable2MFA)
                {
                    await this.Enable2FAAsync();
                    this.State.Push(ILoginRepository.Steps.Step4_Validate2FA);
                }
                else if (step == ILoginRepository.Steps.Step4_Validate2FA)
                {
                    await this.Validate2FAAsync();
                    this.State.Push(ILoginRepository.Steps.Step5_WelcomeToTheApplication);
                    this.WelcomeToTheApplication?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex) 
            {
                this._userFacade.Result = new() { Error = ex.GetMessage(), };
            }
            finally 
            { 
                this.CurrentStepChanged?.Invoke(this, EventArgs.Empty);
                this.RefreshUI?.Invoke(this, EventArgs.Empty);
            }
        }

        public Task LoginAsync()
        {
            var user = this._superRepository.Users.LookupUserInDatabase(this.Username, this.Password);
            if (user != null)
            {
                this.User = user;
                return Task.CompletedTask;
            }

            throw new InvalidOperationException("Combination of username and password does not exist in the database");
        }

        public void Regenerate2FARecoveryCodes()
        {
            this.TOTPRecoveryCodes = this._superRepository.Users.GenerateTOTPRecoveryCodes();
            this.RefreshUI?.Invoke(this, EventArgs.Empty);
        }

        public void Regenerate2FASecret()
        {
            this.User.TOTPSecret = Guid.NewGuid();
            this.RefreshUI?.Invoke(this, EventArgs.Empty);
        }

        public Task ResetAsync()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.NewPassword1 = string.Empty;
            this.NewPassword2 = string.Empty;
            this.TOTPCodeForEnablement = string.Empty;
            this.TOTPCodeForValidation = string.Empty;
            this.TOTPRecoveryCodes = this._superRepository.Users.GenerateTOTPRecoveryCodes();
            this.User = new OrigamiUser();

            this.State.Clear();
            this.State.Push(ILoginRepository.Steps.Step1_ValidateCredentials);

            return Task.CompletedTask;
        }

        public Task Validate2FAAsync()
        {
            var secretBytes = Base32Encoding.ToBytes(this.User.TOTPSecret.ToString());
            var totp = new Totp(secretBytes);
            var valid = totp.VerifyTotp(this.TOTPCodeForValidation, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (valid)
            {
                return Task.CompletedTask;
            }

            throw new InvalidOperationException("Failed to validate 2FA.");
        }

        private void _2FA()
        {
            if (this.User.TOTPSecret == Guid.Empty)
            {
                this._userFacade.Result = new() { Info = _text.Original("You must enable two-factor authentication") };
                this.State.Push(ILoginRepository.Steps.Step3_MustEnable2MFA);
                return;
            }

            this.State.Push(ILoginRepository.Steps.Step4_Validate2FA);
        }
    }
}
