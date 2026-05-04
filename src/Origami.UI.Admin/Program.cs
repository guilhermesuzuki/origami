using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Origami.UI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var app = builder.FoldTheOrigami<Origami.UI.Admin.Components.App>(
    args,
    admin: true,
    inject: () =>
    {
        //adds admin site authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                ValidateIssuerSigningKey = true,
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var key = builder.Configuration.GetUserCookieKey();
                    context.Token = context.Request.Cookies[key];
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    return Task.CompletedTask;
                },
            };
        });

        /*image cropper*/
        builder.Services.AddBootstrapBlazor();

        //kestrel 8GB
        builder.WebHost.ConfigureKestrel(serverOptions => serverOptions.Limits.MaxRequestBodySize = (long)8 * 1024 * 1024 * 1024);
    });

await app.RunAsync();