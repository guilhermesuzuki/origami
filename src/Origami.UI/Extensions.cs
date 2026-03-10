using DeviceDetectorNET;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.Jwt;
using Origami.Core.Validators;
using Origami.UI.Services;
using Serilog;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;

namespace Origami.UI
{
    public static class Extensions
    {
        public static void AddOrigami(this WebApplicationBuilder builder, string[] args, bool admin = false)
        {
            var files = Path.GetFullPath("..\\Origami.Files\\");

            builder.Configuration.AddJsonFile(Path.Combine(files, "dbsettings.json"), false, reloadOnChange: true);
            builder.Configuration.AddJsonFile(Path.Combine(files, $"dbsettings.{builder.Environment.EnvironmentName}.json"), true, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
            builder.Services.AddControllers().AddApplicationPart(typeof(Basic).Assembly);
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            //mudblazor
            builder.Services.AddMudServices();
            builder.Services.AddMemoryCache();

            //adds the http client
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddLocalization();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews();
            builder.Services.AddServerSideBlazor().AddHubOptions(options => { options.MaximumReceiveMessageSize = 16 * 1024 * 1024; });

            //origami connection string
            var origami = builder.Configuration.GetOrigamiConnectionString();

            builder.Services.AddDbContextFactory<OrigamiDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(origami);
            });

            builder.Services.AddDbContextFactory<OrigamiIdentityDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(origami);
            });

            builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<OrigamiIdentityDbContext>();

            builder.Services.AddScoped<OrigamiJwtUserMiddleware>();
            builder.Services.AddScoped<OrigamiLocationMiddleware>();

            builder.Services.AddSingleton<Text>();
            builder.Services.AddSingleton<IAppFacade, AppFacade>(provider => new AppFacade(admin, builder.Environment.EnvironmentName));
            builder.Services.AddSingleton<ISlideRepository, SlideRepository>();
            builder.Services.AddSingleton<IEmailStatusRepository, EmailStatusRepository>();
            builder.Services.AddSingleton<IBackupRestoreRepository, BackupRestoreRepository>();
            builder.Services.AddSingleton<IRepository<OrigamiBackup>, BackupRestoreRepository>();
            builder.Services.AddTransient<IBlogRepository, BlogRepository>();
            builder.Services.AddTransient<IBlogRollRepository, BlogRollRepository>();
            builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
            builder.Services.AddTransient<IDashboardRepository, DashboardRepository>();
            builder.Services.AddTransient<IDirectoryRepository, DirectoryRepository>();
            builder.Services.AddTransient<IEmailRepository, EmailRepository>();
            builder.Services.AddTransient<IFileManagerRepository, FileManagerRepository>();
            builder.Services.AddTransient<IFileRepository, FileRepository>();
            builder.Services.AddTransient<IPageRepository, PageRepository>();
            builder.Services.AddTransient<IPageTitleRepository, PageTitleRepository>();
            builder.Services.AddTransient<IPageViewRepository, PageViewRepository>();
            builder.Services.AddTransient<IPhysicalPageRepository, PhysicalPageRepository>();
            builder.Services.AddTransient<IPhysicalPageViewRepository, PhysicalPageViewRepository>();
            builder.Services.AddTransient<IPingServiceRepository, PingServiceRepository>();
            builder.Services.AddTransient<IPostCategoryRepository, PostCategoryRepository>();
            builder.Services.AddTransient<IPostCommentReactionRepository, PostCommentReactionRepository>();
            builder.Services.AddTransient<IPostCommentRepository, PostCommentRepository>();
            builder.Services.AddTransient<IPostRatingRepository, PostRatingRepository>();
            builder.Services.AddTransient<IPostRepository, PostRepository>();
            builder.Services.AddTransient<IPostTagRepository, PostTagRepository>();
            builder.Services.AddTransient<IPostViewRepository, PostViewRepository>();
            builder.Services.AddTransient<IResumeRepository, ResumeRepository>();
            builder.Services.AddTransient<IRightRepository, RightRepository>();
            builder.Services.AddTransient<IRightRoleRepository, RightRoleRepository>();
            builder.Services.AddTransient<IRoleRepository, RoleRepository>();
            builder.Services.AddTransient<IRssRepository, RssRepository>();
            builder.Services.AddTransient<ISettingRepository, SettingRepository>();
            builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
            builder.Services.AddTransient<ISocialProfileDeleteRepository, SocialProfileDeleteRepository>();
            builder.Services.AddTransient<ISocialProfileRepository, SocialProfileRepository>();
            builder.Services.AddTransient<ISpecialMessageRepository, SpecialMessageRepository>();
            builder.Services.AddTransient<ISpecialPageRepository, SpecialPageRepository>();
            builder.Services.AddTransient<ISpecialPageViewRepository, SpecialPageViewRepository>();
            builder.Services.AddTransient<ISubscriberRepository, SubscriberRepository>();
            builder.Services.AddTransient<ISuperRepository, SuperRepository>();
            builder.Services.AddTransient<ITagRepository, TagRepository>();
            builder.Services.AddTransient<ITrashRepository, TrashRepository>();
            builder.Services.AddTransient<IUserActivityRepository, UserActivityRepository>();
            builder.Services.AddTransient<IUserContentRepository, UserContentRepository>();
            builder.Services.AddTransient<IUserPasswordResetRepository, UserPasswordResetRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddTransient<IUserTrashRepository, UserTrashRepository>();
            builder.Services.AddTransient<IUserViewRepository, UserViewRepository>();
            builder.Services.AddTransient<IVideoCategoryRepository, VideoCategoryRepository>();
            builder.Services.AddTransient<IVideoCommentReactionRepository, VideoCommentReactionRepository>();
            builder.Services.AddTransient<IVideoCommentRepository, VideoCommentRepository>();
            builder.Services.AddTransient<IVideoRatingRepository, VideoRatingRepository>();
            builder.Services.AddTransient<IVideoRepository, VideoRepository>();
            builder.Services.AddTransient<IVideoTagRepository, VideoTagRepository>();
            builder.Services.AddTransient<IVideoViewRepository, VideoViewRepository>();
            builder.Services.AddTransient<IWhatToSeeNextRepository, WhatToSeeNextRepository>();

            builder.Services.AddKeyedSingleton<IIpLocationRepository, IpApiComRepository>(IpApiComRepository.Host);
            builder.Services.AddKeyedSingleton<IIpLocationRepository, IpApiCoRepository>(IpApiCoRepository.Host);
            builder.Services.AddKeyedSingleton<IIpLocationRepository, IpWhoIsRepository>(IpWhoIsRepository.Host);
            builder.Services.AddSingleton<IIpLocationRepository, IpLocationRepository>();

            builder.Services.AddCrud<OrigamiBlog, BlogRepository>();
            builder.Services.AddCrud<OrigamiCategory, CategoryRepository>();
            builder.Services.AddCrud<OrigamiFile, FileManagerRepository>();
            builder.Services.AddCrud<OrigamiPage, PageRepository>();
            builder.Services.AddCrud<OrigamiPost, PostRepository>();
            builder.Services.AddCrud<OrigamiPostCategory, PostCategoryRepository>();
            builder.Services.AddCrud<OrigamiPostComment, PostCommentRepository>();
            builder.Services.AddCrud<OrigamiPostTag, PostTagRepository>();
            builder.Services.AddCrud<OrigamiRole, RoleRepository>();
            builder.Services.AddCrud<OrigamiSettings, SettingsRepository>();
            builder.Services.AddCrud<OrigamiSocialProfile, SocialProfileRepository>();
            builder.Services.AddCrud<OrigamiSpecialMessage, SpecialMessageRepository>();
            builder.Services.AddCrud<OrigamiSpecialPage, SpecialPageRepository>();
            builder.Services.AddCrud<OrigamiTag, TagRepository>();
            builder.Services.AddCrud<OrigamiTrash, TrashRepository>();
            builder.Services.AddCrud<OrigamiUser, UserRepository>();
            builder.Services.AddCrud<OrigamiUserTrash, UserTrashRepository>();
            builder.Services.AddCrud<OrigamiVideo, VideoRepository>();
            builder.Services.AddCrud<OrigamiVideoCategory, VideoCategoryRepository>();
            builder.Services.AddCrud<OrigamiVideoComment, VideoCommentRepository>();
            builder.Services.AddCrud<OrigamiVideoTag, VideoTagRepository>();

            //sets the blog as the primary one
            builder.Services.AddScoped<IUserFacade, UserFacade>(provider =>
            {
                var blog = provider.GetRequiredService<IBlogRepository>();
                return new() { BlogId = blog.GetPrimary().Id };
            });

            //really important workaround
            builder.Services.AddSingleton<IWebRootPath>(provider =>
            {
                var env = provider.GetRequiredService<IWebHostEnvironment>();
                return new WwwRoot(env.WebRootPath);
            });

            builder.Services.AddScoped<CustomHeadContentService>();
            builder.Services.AddHostedService<CacheRefreshServiceFull>();
            builder.Services.AddHostedService<MailConnectivityCheckService>();
            builder.Services.AddSingleton<CircuitHandler, OrigamiCircuitHandler>();
            builder.Services.AddScoped<HtmlRenderer>();

            builder.Services.AddSingleton<IValidator<OrigamiBlog>, OrigamiBlogValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiCategory>, OrigamiCategoryValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiPage>, OrigamiPageValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiPost>, OrigamiPostValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiPostComment>, OrigamiPostCommentValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiSettings>, OrigamiSettingsValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiSpecialMessage>, OrigamiSpecialMessageValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiSpecialPage>, OrigamiSpecialPageValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiVideo>, OrigamiVideoValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiVideoComment>, OrigamiVideoCommentValidator>();
            builder.Services.AddSingleton<IValidator<OrigamiUser>, OrigamiUserValidator>();

            //jwt configuration
            builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<Core.Models.Settings.OpenTelemetry>(builder.Configuration.GetSection("OpenTelemetry"));
            builder.Services.Configure<SocialNetwork>(builder.Configuration.GetSection("SocialNetwork"));

            //gzip compression
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });

            /**/
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownProxies.Clear();
                options.KnownIPNetworks.Clear();
            });

            /*rate limiting*/
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var userId = context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? context.Connection.LocalIpAddress?.ToString() ?? context.Connection.Id;
                    return RateLimitPartition.GetFixedWindowLimiter(
                        userId,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 250, // max requests per second *per user*
                            Window = TimeSpan.FromSeconds(1)
                        });
                });

                // Set status code on rejection
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            var services = builder.Services.BuildServiceProvider();

            //adds the database configuration
            builder.Configuration.AddDatabase(services);

            //adds command line
            builder.Configuration.AddCommandLine(args);
        }

        /// <summary>
        /// Registers CRUD-related services for the specified entity and repository types.
        /// </summary>
        /// <remarks>This method registers the specified repository type as the implementation for various
        /// CRUD-related interfaces, including <see cref="_ICrudCache{TEntity}"/>, <see cref="_ICreateCache{TEntity}"/>,
        /// <see cref="_IReadCache{TEntity}"/>, <see cref="_IUpdateCache{TEntity}"/>, <see
        /// cref="_IDeleteCache{TEntity}"/>, and <see cref="_ISaveCache{TEntity}"/>. Use this method to configure
        /// dependency injection for CRUD operations in your application.</remarks>
        /// <typeparam name="TEntity">The type of the entity. Must implement <see cref="IId"/>.</typeparam>
        /// <typeparam name="TRepository">The type of the repository. Must implement <see cref="_ICrudCache{TEntity}"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddCrud<TEntity, TRepository>(this IServiceCollection services)
            where TEntity : IId
            where TRepository : class, IRepository<TEntity>
        {
            services.AddTransient<IRepository<TEntity>, TRepository>();
            services.AddTransient<ICreateCache<TEntity>, TRepository>();
            services.AddTransient<IReadCache<TEntity>, TRepository>();
            services.AddTransient<IUpdateCache<TEntity>, TRepository>();
            services.AddTransient<IDeleteCache<TEntity>, TRepository>();
            services.AddTransient<ISearch<TEntity>, TRepository>();

            return services;
        }

        public static string Error(this IEnumerable<IdentityError> errors)
        {
            if (errors.Count() > 0)
            {
                var error = new StringBuilder();

                foreach (var row in errors)
                {
                    error.AppendFormat(", {0} => {1}", row.Code, row.Description);
                }

                return error.ToString()[1..].TrimStart();
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the filter expression for EF from a MudBlazor filter definition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static string Filter<T>(this IFilterDefinition<T>? definition)
        {
            if (definition != null)
            {
                switch (definition.Operator)
                {
                    case "contains": return $"{definition.Column!.PropertyName}.Contains(\"{definition.Value}\") == true";
                    case "not contains": return $"{definition.Column!.PropertyName}.Contains(\"{definition.Value}\") == false";
                    case "equals": return $"{definition.Column!.PropertyName}.Equals(\"{definition.Value}\") == true";
                    case "not equals": return $"{definition.Column!.PropertyName}.Equals(\"{definition.Value}\") == false";
                    case "starts with": return $"{definition.Column!.PropertyName}.StartsWith(\"{definition.Value}\") == true";
                    case "ends with": return $"{definition.Column!.PropertyName}.EndsWith(\"{definition.Value}\") == true";
                }
            }

            return string.Empty;
        }

        public static async Task<string> GetBase64Image(this IBrowserFile file)
        {
            long bytesRead = 0;
            const int bufferSize = 1024 * 1024;
            await using Stream stream = file.OpenReadStream(file.Size);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                using MemoryStream memoryStream = new(capacity: (int)file.Size);
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, (int)bytesRead);
                }

                var fileBytes = memoryStream.TryGetBuffer(out ArraySegment<byte> segment) && segment.Array != null
                    ? segment.Array
                    : memoryStream.ToArray();

                return $"data:{file.ContentType};base64,{Convert.ToBase64String(fileBytes)}";
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static DeviceDetector GetDeviceDetector(this HttpRequest httpRequest)
        {
            var userAgent = httpRequest.Headers["User-Agent"]; // change this to the useragent you want to parse
            var headers = httpRequest.Headers.ToDictionary(a => a.Key, a => a.Value.ToArray().FirstOrDefault());
            var clientHints = ClientHints.Factory(headers);  // client hints are optional
            return new DeviceDetector(userAgent, clientHints);
        }

        public static string GetUserCookieKey(this IConfiguration configuration)
        {
            return configuration.GetValue("User:Cookie-Key", OrigamiConstants.Cookie)!;
        }

        public static string Header(this HttpRequest httpRequest, string header)
        {
            return httpRequest.Headers.ContainsKey(header) ? httpRequest.Headers[header].ToString() : string.Empty;
        }

        public static MarkupString Icon(this SocialNetworks network)
        {
            if (network == SocialNetworks.Facebook) return (MarkupString)Icons.Custom.Brands.Facebook;
            if (network == SocialNetworks.GitHub) return (MarkupString)Icons.Custom.Brands.GitHub;
            if (network == SocialNetworks.Google) return (MarkupString)Icons.Custom.Brands.Google;
            if (network == SocialNetworks.GooglePlus) return (MarkupString)"<path d=\"M386.1 228.5c1.8 9.7 3.1 19.4 3.1 32C389.2 370.2 315.6 448 204.8 448c-106.1 0-192-85.9-192-192s85.9-192 192-192c51.9 0 95.1 18.9 128.6 50.3l-52.1 50c-14.1-13.6-39-29.6-76.5-29.6-65.5 0-118.9 54.2-118.9 121.3 0 67.1 53.4 121.3 118.9 121.3 76 0 104.5-54.7 109-82.8H204.8v-66h181.3zm185.4 6.4V179.2h-56v55.7h-55.7v56h55.7v55.7h56v-55.7H627.2v-56h-55.7z\"/>";
            if (network == SocialNetworks.Microsoft) return (MarkupString)Icons.Custom.Brands.Microsoft;
            if (network == SocialNetworks.Twitter) return (MarkupString)Icons.Custom.Brands.Twitter;
            return (MarkupString)"<path d=\"M399 384.2C376.9 345.8 335.4 320 288 320H224c-47.4 0-88.9 25.8-111 64.2c35.2 39.2 86.2 63.8 143 63.8s107.8-24.7 143-63.8zM0 256a256 256 0 1 1 512 0A256 256 0 1 1 0 256zm256 16a72 72 0 1 0 0-144 72 72 0 1 0 0 144z\"/>";
        }

        /// <summary>
        /// Loads the incognito mode from cookies
        /// </summary>
        /// <param name="jsRuntime">JS runtime to retrieve the cookie</param>
        /// <returns></returns>
        public static async Task<bool> IncognitoModeAsync(this IJSRuntime jsRuntime)
        {
            var cookie = await jsRuntime.InvokeAsync<string>("$.cookie", "incognito-mode");
            if (cookie.Has() == false) return false;
            if (cookie == "1" || cookie.Like("true") == true) return true;
            return false;
        }


        /// <summary>
        /// [WorkAround] To log the user out (by simply deleting the authentication cookie)
        /// </summary>
        /// <param name="context"></param>
        public static void Logout_Workaround(this HttpContext context, string cookieKey = ".AspNetCore.Identity.Application")
        {
            context.Response.Cookies.Delete(cookieKey);
        }

        public static MarkupString Markup(this string html)
        {
            return new MarkupString(html);
        }

        public static WebApplication UseOrigami(this WebApplication app, bool admin = false)
        {
            app.UseAuthentication();
            app.UseForwardedHeaders();

            var supportedCultures = OrigamiConstants.AllLanguages().Select(x => x.Name).ToArray();
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("en-US")
                .AddSupportedCultures("en-US")
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(supportedCultures[0]);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() == false)
            {
                app.UseResponseCaching();
                app.UseResponseCompression();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.UseMiddleware<OrigamiJwtUserMiddleware>();
            app.UseMiddleware<OrigamiLocationMiddleware>();

            app.MapRazorPages();
            app.UseMvcWithDefaultRoute();
            app.MapControllers();
            app.UseStaticFiles();
            app.UseRateLimiter();

            if (admin == false)
            {
                // RSS feed endpoint
                app.MapGet("/blogs/{slug}/rss.xml", async (string slug, HttpContext context, IRssRepository rss) =>
                {
                    var oi = context.Request.Scheme + "://" + context.Request.Host.Value;
                    var xml = rss.GetRss(slug, oi);
                    context.Response.ContentType = "application/rss+xml; charset=utf-8";
                    await context.Response.WriteAsync(xml);
                });
            }

            return app;
        }

        public static WebApplication FoldTheOrigami<T>(
            this WebApplicationBuilder builder,
            string[] args,
            bool admin = false,
            Action? inject = null)
        {
            //first thing in the morning
            builder.AddOrigami(args, admin: admin);

            var siteName = builder.Configuration.GetValue<string>("Site:Name");
            var serviceName = $"origami2, {(admin ? "admin:" : "front-end:")} {siteName}";

            /*open telemetry*/
            var openTelemetry = builder.Configuration.GetValue("OpenTelemetry:Enabled", false);
            if (openTelemetry)
            {
                /*OpenTelemetry*/
                var otel = builder.Services.AddOpenTelemetry();

                // Configure OpenTelemetry Resources with the application name
                otel.ConfigureResource(resource => resource.AddService(serviceName: serviceName));

                // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
                otel.WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation();
                    metrics.AddRuntimeInstrumentation();
                    metrics.AddProcessInstrumentation();
                    metrics.AddHttpClientInstrumentation();
                    metrics.AddPrometheusExporter();

                    // Metrics provides by ASP.NET Core in .NET 8
                    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                    metrics.AddMeter("Microsoft.AspNetCore.Http.Connections");
                    metrics.AddMeter("Microsoft.AspNetCore.Routing");
                    metrics.AddMeter("Microsoft.AspNetCore.Diagnostics");
                    metrics.AddMeter("Microsoft.AspNetCore.RateLimiting");
                });

                // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
                otel.WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation();
                    tracing.AddHttpClientInstrumentation();
                    tracing.AddOtlpExporter(otlpOptions =>
                    {
                        // Use IConfiguration directly for Otlp exporter endpoint option.
                        otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue("OpenTelemetry:Endpoint", defaultValue: "http://localhost:4317")!);
                    });
                });
            }

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.WriteTo.Console();
            });

            //kestrel 8MB
            builder.WebHost.ConfigureKestrel(serverOptions => serverOptions.Limits.MaxRequestBodySize = (long)8 * 1024 * 1024);

            /*there's services to inject*/
            inject?.Invoke();

            /*builds and use origami*/
            var app = builder.Build().UseOrigami(admin: admin);

            if (openTelemetry) app.MapPrometheusScrapingEndpoint();

            app.MapRazorComponents<T>().AddInteractiveServerRenderMode();

            return app;
        }
    }
}
