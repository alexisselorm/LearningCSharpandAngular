using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBGList.Attributes;
using MyBGList.Constants;
using MyBGList.GraphQL;
using MyBGList.gRPC;
using MyBGList.Models;
using MyBGList.Swagger;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Logging
    .ClearProviders()
    .AddSimpleConsole()
    .AddDebug()
    //.AddApplicationInsights(telemetry => telemetry.ConnectionString = builder.Configuration["Azure:ApplicationInsights:ConnectionString"], loggerOptions => { })
    ;

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
    lc.Enrich.WithMachineName();
    lc.Enrich.WithThreadId();
    lc.Enrich.WithThreadName();
    lc.WriteTo.File("Logs/log_.log", outputTemplate:
        "{Timestamp:HH:mm:ss} [{Level:u11}]" +
        "[{MachineName} #{ThreadId} - {ThreadName}] in [{SourceContext}] [{ActionName}]" +
        "{Message:lj}{NewLine}{Exception}"
        , rollingInterval: RollingInterval.Day);
    lc.WriteTo.File("Logs/error_.log", outputTemplate:
        "{Timestamp:HH:mm:ss} [{Level:u11}]" +
        "[{MachineName} #{ThreadId} - {ThreadName}]" +
        "{Message:lj}{NewLine}{Exception}", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day);
    lc.WriteTo.MSSqlServer(
        connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "LogEvents",
            AutoCreateSqlTable = true,
        },
        columnOptions: new ColumnOptions()
        {
            AdditionalColumns = new SqlColumn[] {
                new SqlColumn()
                {
                    ColumnName="SourceContext",
                    PropertyName="SourceContext",
                    DataType= System.Data.SqlDbType.NVarChar
                }
            }
        });
},
writeToProviders: true);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin",
        cfg =>
        {
            cfg.AllowAnyOrigin();
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

builder.Services.AddGrpc();

builder.Services.AddIdentity<ApiUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))

    };
});

//If you would like to add a claims based access control. This particular is for a moderator that has a phone number
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("ModeratorWithPolicy", policy => policy.RequireClaim(ClaimTypes.Role, RoleNames.Moderator).RequireClaim(ClaimTypes.MobilePhone));
//});

builder.Services.AddControllers(
    options =>
    {
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
            x => $"The value '{x}' is invalid");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
            x => $"The field '{x}' must be a number");
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (x, y) => $"The value'{x}' is not valid for {y}");
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
            () => "A value is required");

        //Caching Profiles
        options.CacheProfiles.Add("NoCache",
            new CacheProfile() { NoStore = true });
        options.CacheProfiles.Add("Any-60",
            new CacheProfile()
            {
                Location = ResponseCacheLocation.Any,
                Duration = 60
            });
        options.CacheProfiles.Add("Client-120",
                new CacheProfile()
                {
                    Location = ResponseCacheLocation.Client,
                    Duration = 120
                }
        );
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.ParameterFilter<SortColumnFilter>();
    options.ParameterFilter<SortOrderFilter>();

    //Options to force our API to use Auth tokens
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    //options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type=ReferenceType.SecurityScheme,
    //                Id="Bearer"
    //            }
    //        },
    //        Array.Empty<string>()
    //    }
    //});

    options.OperationFilter<AuthRequirementFilter>();
    options.DocumentFilter<CustomDocumentFilter>();
    options.RequestBodyFilter<PasswordRequestFilter>();
    options.SchemaFilter<CustomKeyValueFilter>();
});


builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 34 * 1024 * 1024;
    options.SizeLimit = 50 * 1024 * 1024;
});

builder.Services.AddMemoryCache();

//Unccomment if you want to use sql server caching
//builder.Services.AddDistributedSqlServerCache(options =>
//{
//    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    options.SchemaName = "dbo";
//    options.TableName = "AppCache";
//});

//Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler(action =>
    {
        action.Run(async context =>
        {
            var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();


            var details = new ProblemDetails();
            details.Detail = exceptionHandler?.Error.Message;
            details.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            details.Status = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(JsonSerializer.Serialize(details));
        });
    });

app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.Use((context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
    {
        NoCache = true,
        NoStore = true
    };
    return next.Invoke();
});

app.MapGraphQL();
app.MapGrpcService<GrpcService>();


// Minimal API
app.MapGet("/error",
    [EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] (HttpContext context) =>
    {
        //TODO: logging sending notifications ,etc
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();


        var details = new ProblemDetails();
        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
        details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        details.Status = StatusCodes.Status500InternalServerError;
        app.Logger.LogError(CustomLogEvents.Error_Get, exceptionHandler?.Error, "An unhandled exception occurred");
        return Results.Problem(details);
    });

app.MapGet("/error/test",
    [EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] () =>
    { throw new Exception("test"); });

app.MapGet("/cod/test",
    [EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] () =>
    Results.Text("<script>" +
        "window.alert('Your client supports JavaScript!" +
        "\\r\\n\\r\\n" +
        $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
        "\\r\\n" +
        "Client time (UTC): ' + new Date().toISOString());" +
        "</script>" +
        "<noscript>Your client does not support JavaScript</noscript>",
        "text/html"));

app.MapGet("/cache/test/1", (HttpContext context) =>
{
    context.Response.Headers["cache-control"] = "no-cache, no-store";
    return Results.Ok();
});

app.MapGet("/cache/test/2", (HttpContext context) =>
{
    return Results.Ok();
});



app.MapGet("/auth/test/1", [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Auth test #1 (authenticate users)", Description = "Returns 200 - OK if called by an authenticated user regardless of it roles")][Authorize][ResponseCache(CacheProfileName = "NoCache")] (HttpContext context) =>
{
    return Results.Ok("You are authorized!");
});
app.MapGet("/auth/test/mod", [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Auth test #2 (authenticate moderators)", Description = "Returns 200 - OK if called by an authenticated user with mod or admin privileges")][Authorize(Roles = RoleNames.Moderator)][ResponseCache(CacheProfileName = "NoCache")] (HttpContext context) =>
{
    return Results.Ok("You are authorized!");
});
app.MapGet("/auth/test/admin", [SwaggerOperation(Tags = new[] { "Auth" }, Summary = "Auth test #3 (authenticate Admin)", Description = "Returns 200 - OK if called by an authenticated admin")][Authorize(Roles = RoleNames.Administrator)][ResponseCache(CacheProfileName = "NoCache")] (HttpContext context) =>
{
    return Results.Ok("You are authorized!");
});

// Controllers
app.MapControllers().RequireCors("AnyOrigin");

app.Run();
