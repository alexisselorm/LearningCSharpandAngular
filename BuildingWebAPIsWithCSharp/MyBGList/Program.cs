using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Attributes;
using MyBGList.Models;

var builder = WebApplication.CreateBuilder(args);

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
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ParameterFilter<SortColumnFilter>();
    options.ParameterFilter<SortOrderFilter>();
});

builder.Services.Configure<ApiBehaviorOptions>(
    options =>
    {
        options.SuppressModelStateInvalidFilter = true;
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
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

// Minimal API
app.MapGet("/error",
    [EnableCors("AnyOrigin")]
[ResponseCache(NoStore = true)] () =>
    Results.Problem());

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

// Controllers
app.MapControllers().RequireCors("AnyOrigin");

app.Run();
