using Common;
using Common.Utilities;
using Data;
using Data.Repositories;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Entities.Common.ViewModels;
using Entities.Models.Roles;
using Entities.Models.User;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using Services.Interfaces;
using Services.Interfaces.Services;
using StackExchange.Redis;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using WebFramework.Configuration.Swagger;

namespace WebFramework.Configuration
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            var logger = LogManager
                .Setup()
                .LoadConfigurationFromFile("nlog.config")
                .GetCurrentClassLogger();

            try
            {
                if (builder is null)
                    throw new ArgumentNullException(nameof(builder));

                ConfigLogging(builder);

                var configuration = builder.Configuration;

                #region   AddJwtAuthentication  

                // it is for ==>  be able to inject in other classes 
                builder.Services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

                // to get jwtSettings from  appsettings.Development 
                var _jwtSetting = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

                //builder.Services.AddJwtAuthentication(_jwtSetting);
                AddJwtAuthentication(builder.Services, _jwtSetting);

                #endregion

                builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                SetupNlog(builder);

                AddAppDbContext(builder, configuration);

                AddRedis(builder, configuration);
                
                AddRedisDb(builder, configuration);

                AddHangFire(builder, configuration);

                AddMvcAndJsonOptions(builder);

                AddMinimalMvc(builder);

                AddAppServices(builder);

                //AddCustomApiVersioning(builder);

                AddSwagger(builder);

                AddAppHsts(builder);

#if !DEBUG
           //ApplyRemainingMigrations(builder); // TODO : بررسی بشه امکان اجرا در این حالت داره یا نه و گرنه باید به قسمت middleware برده بشه
#endif
                return builder;
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                throw;
            }
        }
        private static void ConfigLogging(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddNLogWeb();
            builder.Host.UseNLog();
        }

        private static void AddAppDbContext(WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer("Data Source =DESKTOP-96I4231; Initial Catalog=EstateProject; Integrated Security=true;Trust Server Certificate=true;");
            });
        }
        private static void AddRedis(WebApplicationBuilder builder, IConfiguration configuration)
        {
            string redisConnectionString = "redis-18454.c1.us-central1-2.gce.cloud.redislabs.com:18454,password=ghBbFNfOWuTXPo9RalT1XBPhjJCydUXj,ssl=True,abortConnect=False,defaultDatabase=0";
            ConfigurationOptions redisConfig = ConfigurationOptions.Parse(redisConnectionString);
            IConnectionMultiplexer connection = ConnectionMultiplexer.Connect(redisConfig);

            // Register the connection as a singleton service
            builder.Services.AddSingleton(connection);
        }
        public static void AddHangFire(WebApplicationBuilder builder, IConfiguration configuration)
        {

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(@"Data Source =DESKTOP-96I4231; Initial Catalog=EstateProject;
                Integrated Security=true;Trust Server Certificate=true;"));

            // Configure Hangfire dashboard
            builder.Services.AddHangfireServer(); // This line starts the Hangfire background processing server

        } 
        public static void AddRedisDb(WebApplicationBuilder builder, IConfiguration configuration)
        {

            var cacheSettingConfiguration = builder.Configuration.GetSection("CacheSettings");
            builder.Services.Configure<CacheSettings>(cacheSettingConfiguration);

        }

        private static void AddSwagger(WebApplicationBuilder builder)
        {
            Assert.NotNull(builder.Services, nameof(builder.Services));

            //Add services to use Example Filters in swagger
            //services.AddSwaggerExamples();
            //Add services and configuration to use swagger
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
                //show controller XML comments like summary
                options.IncludeXmlComments(xmlDocPath, true);
                //options.OperationFilter<FormFileSwaggerFilter>();
                //options.EnableAnnotations();
                options.UseInlineDefinitionsForEnums();
                //options.DescribeAllParametersInCamelCase();
                //options.DescribeStringEnumsInCamelCase();
                //options.UseReferencedDefinitionsForEnums();
                //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                //options.IgnoreObsoleteActions();
                //options.IgnoreObsoleteProperties();
                //options.CustomSchemaIds(type => type.FullName);

                options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API V1" });
                //options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2" });

                #region Filters
                ////Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
                //options.ExampleFilters();

                ////Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                options.OperationFilter<FileUploadOperation>(); // Add this line to enable file upload
                                                                ////Set summary of action if not already set
                options.OperationFilter<ApplySummariesOperationFilter>();

                //#region Add UnAuthorized to Response
                ////Add 401 response and security requirements (Lock icon) to actions that need authorization
                options.OperationFilter<UnauthorizedResponsesOperationFilter>(false, "Bearer");
                //#endregion

                #region security for swagger

                //        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //        {
                //            In = ParameterLocation.Header,
                //            Description = "Please enter a valid token",
                //            Name = "Authorization",
                //            Type = SecuritySchemeType.Http,
                //            BearerFormat = "JWT",
                //            Scheme = "Bearer"
                //        });
                //        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type=ReferenceType.SecurityScheme,
                //                Id="Bearer"
                //            }
                //        },
                //        new string[]{}
                //    }
                //});

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("https://localhost:7076/api/Account/Login"),
                            Scopes = new Dictionary<string, string>
            {
                {"read", "Read access to protected resources."},
                {"write", "Write access to protected resources."},
            }
                        }
                    }
                });

                #endregion

                //#region Add Jwt Authentication
                //Add Lockout icon on top of swagger ui page to authenticate

                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "bearer"
                //});

                ////options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                ////{
                ////    {"Bearer", new string[] { }}
                ////});
                ///
                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        Implicit = new OpenApiOAuthFlow
                //        {
                //            AuthorizationUrl = new Uri("https://localhost:7188/api/v1/User/login"),
                //            Scopes = new Dictionary<string, string>
                //    {
                //        {"read", "Read access to protected resources."},
                //        {"write", "Write access to protected resources."},
                //    }
                //        }
                //    }
                //});
                #endregion

                #region Versioning
                //// Remove version parameter from all Operations
                //options.OperationFilter<RemoveVersionParameters>();

                //////set version "api/v{version}/[controller]" from current swagger doc verion
                //options.DocumentFilter<SetVersionInPaths>();

                //////Seperate and categorize end-points by doc version
                //options.DocInclusionPredicate((docName, apiDesc) =>
                //{
                //    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                //    var versions = methodInfo.DeclaringType
                //        .GetCustomAttributes<ApiVersionAttribute>(true)
                //        .SelectMany(attr => attr.Versions);

                //    return versions.Any(v => $"v{v.ToString()}" == docName);
                //});
                #endregion

                //If use FluentValidation then must be use this package to show validation in swagger (MicroElements.Swashbuckle.FluentValidation)
                //options.AddFluentValidationRules();

            });
        }

        private static void SetupNlog(WebApplicationBuilder builder)
        {

            ILoggerFactory loggerFactory = new LoggerFactory();
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            //loggerFactory.AddNLog().ConfigureNLog("nlog.config");
#if DEBUG
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();
            builder.Logging.AddEventLog();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
#endif
            builder.Logging.ClearProviders();
            builder.Logging.AddNLogWeb();
            builder.Host.UseNLog();

        }

        private static void AddMvcAndJsonOptions(WebApplicationBuilder builder)
        {
            builder.Services
                             .AddControllers()
                             .AddJsonOptions(options =>
                             {
                                 options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                             })
                             .AddNewtonsoftJson(options =>
                             {
                                 options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                 options.SerializerSettings.Converters.Add(new StringEnumConverter());
                                 options.SerializerSettings.Culture = new CultureInfo("en");
                                 options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                                 options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                                 options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                                 options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                                 options.SerializerSettings.ContractResolver = new DefaultContractResolver
                                 {
                                     NamingStrategy = new CamelCaseNamingStrategy()
                                 };
                                 options.AllowInputFormatterExceptionMessages = true;
                             });

        }

        public static void AddMinimalMvc(WebApplicationBuilder builder)
        {
            //https://github.com/aspnet/AspNetCore/blob/0303c9e90b5b48b309a78c2ec9911db1812e6bf3/src/Mvc/Mvc/src/MvcServiceCollectionExtensions.cs
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter()); //Apply AuthorizeFilter as global filter to all actions

                //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
                //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
                //Use this filter when use cookie 
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                //options.UseYeKeModelBinder();
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.Converters.Add(new StringEnumConverter());
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //option.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            builder.Services.AddSwaggerGenNewtonsoftSupport();

            #region Old way (We don't need this from ASP.NET Core 3.0 onwards)
            ////https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc/MvcServiceCollectionExtensions.cs
            //services.AddMvcCore(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter());

            //    //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
            //    //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
            //    //Use this filter when use cookie 
            //    //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

            //    //options.UseYeKeModelBinder();
            //})
            //.AddApiExplorer()
            //.AddAuthorization()
            //.AddFormatterMappings()
            //.AddDataAnnotations()
            //.AddJsonOptions(option =>
            //{
            //    //option.JsonSerializerOptions
            //})
            //.AddNewtonsoftJson(/*option =>
            //{
            //    option.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //    option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //}*/)

            ////Microsoft.AspNetCore.Mvc.Formatters.Json
            ////.AddJsonFormatters(/*options =>
            ////{
            ////    options.Formatting = Newtonsoft.Json.Formatting.Indented;
            ////    options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            ////}*/)

            //.AddCors()
            //.SetCompatibilityVersion(CompatibilityVersion.Latest); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            #endregion
        }

        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;


            }).AddJwtBearer(options =>
            {
                var secretkey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptKey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptKey),
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;

                // these are for SecurityStamp of user
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("Authentication failed.", context.Exception);

                        if (context.Exception != null)
                            throw new UnauthorizedAccessException("Authentication failed.");


                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        //var applicationSignInManager = context.HttpContext.RequestServices.GetRequiredService<IApplicationSignInManager>();
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IRepository<User>>();
                        var userroleRepository = context.HttpContext.RequestServices.GetRequiredService<IRepository<UserRoles>>();
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (!securityStamp.HasValue())
                            context.Fail("This token has no secuirty stamp");

                        //Find user and token from database and perform your custom validation
                        var userId = claimsIdentity.GetUserId<int>();
                        var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                        if (user.SecurityStamp != Guid.Parse(securityStamp))
                            context.Fail("Token secuirty stamp is not valid.");

                        //var validatedUser = await applicationSignInManager.ValidateSecurityStampAsync(context.Principal);
                        //if (validatedUser == null)
                        //    context.Fail("Token secuirty stamp is not valid.");

                        //if (!user.IsActive)
                        //    context.Fail("User is not active.");
                        //throw new UnauthorizedAccessException("User is not active.");

                        //await userService.UpdateLastLoginDateAsync(userId, context.HttpContext.RequestAborted);
                    },
                    OnChallenge = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                        if (context.AuthenticateFailure != null)
                            throw new UnauthorizedAccessException("An error occurred");
                        throw new UnauthorizedAccessException("You are unauthorized to access this resource.");

                        //return Task.CompletedTask;
                    }
                };
            });
        }

        //private static void AddCustomApiVersioning(WebApplicationBuilder builder)
        //{
        //    builder.Services.AddApiVersioning(options =>
        //    {
        //        //url segment => {version}
        //        options.DefaultApiVersion = new ApiVersion(1, 0);
        //        options.AssumeDefaultVersionWhenUnspecified = true;
        //        options.ReportApiVersions = true;

        //        //ApiVersion.TryParse("1.0", out var version10);
        //        //ApiVersion.TryParse("1", out var version1);
        //        //var a = version10 == version1;

        //        //options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
        //        // api/posts?api-version=1

        //        //options.ApiVersionReader = new UrlSegmentApiVersionReader();
        //        // api/v1/posts

        //        //options.ApiVersionReader = new HeaderApiVersionReader(new[] { "Api-Version" });
        //        // header => Api-Version : 1

        //        //options.ApiVersionReader = new MediaTypeApiVersionReader()

        //        //options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"), new UrlSegmentApiVersionReader())
        //        // combine of [querystring] & [urlsegment]
        //    });

        //}
        private static void AddAppServices(WebApplicationBuilder builder)
        {

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IAdvertiseService, AdvertiseService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddTransient<IMemoryService, MemoryService>();
            builder.Services.AddTransient<ICountOnlineUsersService, CountOnlineUsersService>();
            builder.Services.AddAutoMapper(typeof(WebApplication));
            builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        private static void AddAppHsts(WebApplicationBuilder builder)
        {
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                //options.ExcludedHosts.Add("example.com");
                //options.ExcludedHosts.Add("www.example.com");
            });
        }

        private static void ApplyRemainingMigrations(WebApplicationBuilder builder)
        {
            var serviceScopeFactory = builder.Services.BuildServiceProvider().GetService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }

}
