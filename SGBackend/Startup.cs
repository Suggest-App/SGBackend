using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;
using Quartz;
using SecretsProvider;
using SGBackend.Connector;
using SGBackend.Connector.Spotify;
using SGBackend.Entities;
using SGBackend.Models;
using SGBackend.Provider;
using SGBackend.Service;

namespace SGBackend;

public class Startup
{
    public void ConfigureServices(WebApplicationBuilder builder)
    {
        // add and build tempt services for using secrets in configs
        // needs to be first (used in service extensions)
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDevSecretsProvider();
        }
        
        if(builder.Environment.IsProduction() || builder.Environment.IsDevStage())
        {
            builder.Services.AddEnvSecretsProvider();
        }
        
        var tempProvider = builder.Services.BuildServiceProvider();
        var secretsProvider = tempProvider.GetRequiredService<ISecretsProvider>();

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        builder.Services.AddFeatureManagement();
        builder.Services.AddExternalApiClients();
        builder.Services.AddDbContext<SgDbContext>();
        builder.Services.AddScoped<SpotifyConnector>();
        builder.Services.AddScoped<StateService>();
        builder.Services.AddScoped<TransferService>();
        builder.Services.AddSingleton<JwtProvider>();
        builder.Services.AddScoped<RandomUserService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddSingleton<AccessTokenProvider>();
        builder.Services.AddSingleton<MatchingService>();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UsePersistentStore(o =>
            {
                o.UsePostgres(secretsProvider.GetSecret<Secrets>().DBConnectionString);
                o.UseJsonSerializer();
            });
        });

        builder.Services.AddQuartzHostedService(o => { o.WaitForJobsToComplete = true; });

        // configure jwt validation using tokenprovider
        builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<JwtProvider>((options, provider) =>
                options.TokenValidationParameters =
                    provider.GetJwtValidationParameters(secretsProvider.GetSecret<Secrets>().JwtKey));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.LoginPath = "/signin";
            options.LogoutPath = "/signout";
        }).AddJwtBearer().AddSpotify(options =>
        {
            options.ClientId = secretsProvider.GetSecret<Secrets>().SpotifyClientId;
            options.ClientSecret = secretsProvider.GetSecret<Secrets>().SpotifyClientSecret;
            options.Scope.Add("user-read-recently-played");
            // TODO: wait for spotify dashboard fix, then implement create playlist func for spotify
            //options.Scope.Add("user-read-private");
            //options.Scope.Add("user-read-read-email");
            //options.Scope.Add("playlist-modify-private");

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = async context =>
                {
                    // this means the user logged in successfully at spotify
                    var spotifyConnector = context.HttpContext.RequestServices.GetRequiredService<SpotifyConnector>();
                    var tokenProvider = context.HttpContext.RequestServices.GetRequiredService<JwtProvider>();
                    var accessTokenProvider =
                        context.HttpContext.RequestServices.GetRequiredService<AccessTokenProvider>();

                    var handleResult = await spotifyConnector.HandleUserLoggedIn(context);
                    var dbUser = handleResult.User;

                    // save cached token to store
                    if (context.AccessToken != null && context.ExpiresIn.HasValue)
                        accessTokenProvider.InsertTokenIntoCache(dbUser.Id, new AccessToken
                        {
                            Fetched = DateTime.Now,
                            Token = context.AccessToken,
                            ExpiresIn = context.ExpiresIn.Value
                        });

                    // write spotify access token to jwt
                    context.Response.Cookies.Append("jwt", tokenProvider.GetJwt(dbUser));

                    // cookie is still signed in but its irrelevant since we are using
                    // jwt scheme for auth

                    if (!handleResult.ExistedPreviously)
                    {
                        var paralellAlgo =
                            context.HttpContext.RequestServices.GetRequiredService<MatchingService>();

                        var history = await spotifyConnector.FetchAvailableContentHistory(dbUser);

                        if (history != null)
                            // only with valid token
                            await paralellAlgo.Process(dbUser.Id, history);
                    }
                }
            };
        });

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "SG Api", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
    }

    public async Task Configure(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        // init db firstly (needed by following operations)
        var dbContext = services.GetRequiredService<SgDbContext>();
        await dbContext.Database.MigrateAsync();

        // fetch state once
        var stateManager = services.GetRequiredService<StateService>();
        var state = await stateManager.GetState();

        // generale stage independent inits

        // quartz & job init
        var secrets = services.GetRequiredService<ISecretsProvider>().GetSecret<Secrets>();
        var transferService = services.GetRequiredService<TransferService>();

        if (secrets.InitializeFromTarget != null && secrets.InitializeTargetToken != null &&
            !state.InitializedFromTarget)
        {
            // initialize from prod
            await transferService.ImportFromTarget(secrets.InitializeFromTarget, secrets.InitializeTargetToken);
            state.InitializedFromTarget = true;
            await dbContext.SaveChangesAsync();
        }

        if (!state.QuartzApplied)
        {
            // quartz
            var quartzTables = File.ReadAllText("generateQuartzTables.sql");
            await dbContext.Database.ExecuteSqlRawAsync(quartzTables);
            state.QuartzApplied = true;
            await dbContext.SaveChangesAsync();
        }
        
        // prod inits
        if (app.Environment.IsProduction())
        {
            app.Use(async (context, next) =>
            {
                context.Request.Host = new HostString("suggest-app.com");
                context.Request.Scheme = "https";
                await next();
            });

            if (!state.GroupedFetchJobInstalled)
            {
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

                // fetch job
                var job = JobBuilder.Create<SpotifyGroupedFetchJob>()
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("groupedFetchJob", "fetch")
                    .WithSchedule(SimpleScheduleBuilder.RepeatHourlyForever(4))
                    .StartNow()
                    .Build();

                var scheduler = await schedulerFactory.GetScheduler();
                await scheduler.ScheduleJob(job, trigger);

                state.GroupedFetchJobInstalled = true;
                await dbContext.SaveChangesAsync();
            }
        }

        // dev inits
        if (app.Environment.IsDevelopment())
        {
            // overwrite host for oauth redirect
            // dev fe is running on different port, vite.config.js proxies
            // the relevant oauth requests to the dev running backend
            app.Use(async (context, next) =>
            {
                // localhost:5173 is the default port for serving the frontend with 'npm run dev'
                context.Request.Host = new HostString("localhost:5173");
                await next();
            });

            var config = services.GetRequiredService<IConfiguration>();

            var generateRandomUsers = config.GetValue<bool?>("GenerateRandomUsers");
            if (generateRandomUsers != null && generateRandomUsers.Value && !state.RandomUsersGenerated)
            {
                var rndService = services.GetRequiredService<RandomUserService>();
                rndService.GenerateXRandomUsersAndCalc(5).Wait();
                state.RandomUsersGenerated = true;
                await dbContext.SaveChangesAsync();
            }
        }

        // default usings
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

public static class IWebHostEnvironmentExtensions
{
    public static bool IsDevStage(this IWebHostEnvironment env)
    {
        return env.EnvironmentName == "DevStage";
    }
}