using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Validators;
using Origami.Core;
using Origami.UI;
using Origami.UI.FrontEnd.Components;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var app = builder.FoldTheOrigami<App>(
    args,
    admin: false,
    inject: () =>
    {
        builder.Services
            .AddAuthentication()
            //facebook settings
            .AddFacebook(options =>
            {
                options.AccessDeniedPath = "/oops/facebook";
                options.AppId = builder.Configuration["SocialNetwork:Facebook:AppId"]!;
                options.AppSecret = builder.Configuration["SocialNetwork:Facebook:AppSecret"]!;
                options.CallbackPath = builder.Configuration["SocialNetwork:Facebook:CallbackPath"]!;
                options.SaveTokens = true;
            })
            //google settings
            .AddGoogle(options =>
            {
                options.AccessDeniedPath = "/oops/google";
                options.CallbackPath = builder.Configuration["SocialNetwork:Google:CallbackPath"]!;
                options.ClientId = builder.Configuration["SocialNetwork:Google:ClientId"]!;
                options.ClientSecret = builder.Configuration["SocialNetwork:Google:ClientSecret"]!;
                options.SaveTokens = true;
            })
            //github settings
            .AddGitHub(options =>
            {
                options.CallbackPath = builder.Configuration["SocialNetwork:GitHub:CallbackPath"]!;
                options.ClientId = builder.Configuration["SocialNetwork:GitHub:ClientId"]!;
                options.ClientSecret = builder.Configuration["SocialNetwork:GitHub:ClientSecret"]!;
                options.AccessDeniedPath = "/oops/github";
                options.SaveTokens = true;

                // Grants access to read a user's profile data.
                // https://docs.github.com/en/developers/apps/building-oauth-apps/scopes-for-oauth-apps
                options.Scope.Add("read:user");

                // Optional
                // if you need an access token to call GitHub Apis
                options.Events.OnCreatingTicket += context =>
                {
                    if (context.AccessToken.Has() == true)
                    {
                        context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
                    }
                    return Task.CompletedTask;
                };
            })
            //microsoft settings
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, "Microsoft", options =>
            {
                builder.Configuration.Bind("SocialNetwork:Microsoft", options);
                options.AccessDeniedPath = "/oops/microsoft";
                options.Authority = "https://login.microsoftonline.com/common/v2.0";
                options.CallbackPath = builder.Configuration["SocialNetwork:Microsoft:CallbackPath"]!;
                options.ResponseType = "code";
                options.UseTokenLifetime = false;

                var scopes = "openid profile email User.Read".Split(' ');
                scopes.Each(options.Scope.Add);

                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
                options.TokenValidationParameters.SaveSigninToken = true;

                // IMPORTANT: /common is not an issuer.
                options.TokenValidationParameters.ValidIssuer = null;
                options.TokenValidationParameters.ValidIssuers = null;

                options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.GetAadIssuerValidator(options.Authority).Validate;
            });
    });

await app.RunAsync();