using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.Jwt;
using OtpNet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Origami.UI
{
    public class LoginRules : ILoginRules
    {
        protected readonly IConfiguration _configuration;
        protected readonly IJSRuntime _jsRuntime;
        protected readonly NavigationManager _ghostOfTheNavigator;
        protected readonly IOptions<JwtConfiguration> _jwtConfiguration;
        protected readonly ISuperRepository _superRepository;
        protected readonly Text _text;
        protected readonly IUserFacade _userFacade;

        public LoginRules(
            IConfiguration configuration,
            IJSRuntime jsRuntime,
            NavigationManager navigationManager,
            IOptions<JwtConfiguration> jwtConfiguration, 
            ISuperRepository superRepository, 
            IUserFacade userFacade, 
            Text text 
            )
        {
            _configuration = configuration;
            _ghostOfTheNavigator = navigationManager;
            _jsRuntime = jsRuntime;
            _jwtConfiguration = jwtConfiguration;
            _superRepository = superRepository;
            _text = text;
            _userFacade = userFacade;

            this.TOTPRecoveryCodes = _superRepository.Users.GenerateTOTPRecoveryCodes();

            this.ResetAsync().Wait();
        }

        public event EventHandler CurrentStepChanged = null!;
        public event EventHandler RefreshUI = null!;

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
                if (this.User.NewPassword1.Has() == false) return true;
                if (this.User.NewPassword2.Has() == false) return true;
                return false;
            }
        }

        public Stack<ILoginRules.Steps> State { get; } = new Stack<ILoginRules.Steps>();

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
                this.User.NewPassword1,
                this.User.NewPassword2);

            try
            {
                if (hub.Ok == false)
                {
                    throw new InvalidOperationException("Failed to change password.");
                }
            }
            finally
            {
                this._userFacade.Result = hub;
            }
        }

        public void Clear2FA()
        {
            this.TOTPCodeForValidation = string.Empty;
        }

        public void ClearChangePassword()
        {
            this.User.NewPassword1 = string.Empty;
            this.User.NewPassword2 = string.Empty;
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
            var recoveryCodesInSHA256 = string.Join(",", this.TOTPRecoveryCodes.Select(x => x.SHA256Hash()));

            if (valid)
            {
                this.User.TOTPRecoveryCodes = recoveryCodesInSHA256;
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

            throw new InvalidOperationException("Failed to enable 2FA");
        }

        public string GenerateJwtToken(OrigamiUser user)
        {
            //starts generating the JWT token
            var issuer = _jwtConfiguration.Value.Issuer;
            var audience = _jwtConfiguration.Value.Audience;
            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Value.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    ]),
                Expires = DateTime.UtcNow.AddMonths(6),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        public ILoginRules.Steps GetCurrentStep()
        {
            if (State.Count == 0) return ILoginRules.Steps.Step1_ValidateCredentials;
            return State.Peek();
        }

        public Task GoBackAsync()
        {
            this.State.Clear();
            this.State.Push(ILoginRules.Steps.Step1_ValidateCredentials);
            this.RefreshUI?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task GoNextAsync()
        {
            try
            {
                var step = this.GetCurrentStep();
                if (step == ILoginRules.Steps.Step1_ValidateCredentials)
                {
                    await this.LoginAsync();

                    if (this.User.MustChangePassword == true)
                    {
                        this._userFacade.Result = new() { Info = _text.Original("You must change your password") };
                        this.State.Push(ILoginRules.Steps.Step2_MustChangePassword);
                        return;
                    }

                    this._2FA();
                }
                else if (step == ILoginRules.Steps.Step2_MustChangePassword)
                {
                    await this.ChangePasswordAsync();
                    this._2FA();
                }
                else if (step == ILoginRules.Steps.Step3_MustEnable2MFA)
                {
                    await this.Enable2FAAsync();
                    this.State.Push(ILoginRules.Steps.Step4_Validate2FA);
                }
                else if (step == ILoginRules.Steps.Step4_Validate2FA)
                {
                    await this.Validate2FAAsync();
                    this.State.Push(ILoginRules.Steps.Step5_WelcomeToTheApplication);
                    await this.WelcomeToTheApplicationAsync();
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

        public async Task LoginAsync()
        {
            var user = this._superRepository.Users.LookupUserInDatabase(this.Username, this.Password);
            if (user != null)
            {
                this.User = user;
                return;
            }

            await Task.Delay(2000);
            throw new InvalidOperationException("Combination of username and password does not exist in the database");
        }

        public void Regenerate2FARecoveryCodes()
        {
            this.TOTPRecoveryCodes = this._superRepository.Users.GenerateTOTPRecoveryCodes();
            this.RefreshUI?.Invoke(this, EventArgs.Empty);
        }

        public void Regenerate2FASecret()
        {
            this.User.GenerateRandomTOTPSecret();
            this.RefreshUI?.Invoke(this, EventArgs.Empty);
        }

        public Task ResetAsync()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.User.NewPassword1 = string.Empty;
            this.User.NewPassword2 = string.Empty;
            this.TOTPCodeForEnablement = string.Empty;
            this.TOTPCodeForValidation = string.Empty;
            this.TOTPRecoveryCodes = this._superRepository.Users.GenerateTOTPRecoveryCodes();
            this.User = new OrigamiUser();

            this.State.Clear();
            this.State.Push(ILoginRules.Steps.Step1_ValidateCredentials);

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

            var recoveryCodeValid = this.User.ConsumeTOTPRecoveryCode(this.TOTPCodeForValidation);
            if (recoveryCodeValid)
            {
                this._superRepository.Users.SmartSave(this.User.GetContext(), false);
                return Task.CompletedTask;
            }

            throw new InvalidOperationException("Failed to validate 2FA");
        }

        public async Task WelcomeToTheApplicationAsync()
        {
            this._userFacade.UserId = this.User.Id;
            this._superRepository.Users.UpdateCache(this.User);

            var token = this.GenerateJwtToken(this.User);
            await this._jsRuntime.InvokeVoidAsync("$.cookie", this._configuration.GetUserCookieKey(), token, new { path = "/", expires = 365, });

            var returnUrl = this._ghostOfTheNavigator.Uri.QueryString("returnUrl");
            this._ghostOfTheNavigator.NavigateTo(returnUrl.Has() ? returnUrl : "/", true);
        }

        private void _2FA()
        {
            if (this.User.TOTPSecret.Has() == false || this.User.TOTPSecret == Guid.Empty.ToString())
            {
                this.User.GenerateRandomTOTPSecret();
                this._userFacade.Result = new() { Info = _text.Original("You must enable two-factor authentication") };
                this.State.Push(ILoginRules.Steps.Step3_MustEnable2MFA);
                return;
            }

            this.State.Push(ILoginRules.Steps.Step4_Validate2FA);
        }
    }
}
