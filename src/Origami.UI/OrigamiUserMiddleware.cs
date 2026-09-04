using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Origami.UI
{
    public class OrigamiUserMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IOptionsSnapshot<JwtConfiguration> _jwtConfiguration;
        private readonly IAppFacade _appFacade;
        private readonly IUserRepository _userRepository;

        public OrigamiUserMiddleware(
            IAppFacade appFacade,
            IConfiguration configuration,
            IOptionsSnapshot<JwtConfiguration> jwtConfiguration,
            IUserRepository userRepository)
        {
            _appFacade = appFacade;
            _configuration = configuration;
            _jwtConfiguration = jwtConfiguration;
            _userRepository = userRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_appFacade.Admin.GetValueOrDefault() == false)
            {
                await next(context);
                return;
            }

            try
            {
                var cookieKey = _configuration.GetUserCookieKey();
                if (context.Request.Cookies.TryGetValue(cookieKey, out var token))
                {
                    var key = Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Key);
                    var validationParams = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = _jwtConfiguration.Value.Audience,
                        ValidIssuer = _jwtConfiguration.Value.Issuer,
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
                    if (principal.Identity?.IsAuthenticated == true)
                    {
                        var nameId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (nameId != null && Guid.TryParse(nameId, out var id) == true)
                        {
                            context.Items["loggedin-admin-user"] = _userRepository.ReadFromCache().Id(id) ?? new();
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors and continue without user
            }

            await next(context);
        }
    }
}
