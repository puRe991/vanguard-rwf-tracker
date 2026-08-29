using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VanguardTracker.Api;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.Hubs;
using VanguardTracker.Api.Services;
using VanguardTracker.Api.WarcraftLogs;

var builder = WebApplication.CreateBuilder(args);

// Lokale, nicht eingecheckte Overrides (Connection-String, JWT-Key, WCL-Credentials).
// Bewusst nach den appsettings.*.json geladen, damit sie gewinnen; per .gitignore
// ausgeschlossen. Siehe README, Abschnitt "Backend starten".
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Vanguard API",
        Version = $"v{AppInfo.Version}",
    });
});
builder.Services.AddSignalR();

builder.Services.AddDbContext<VanguardDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.Configure<WarcraftLogsOptions>(
    builder.Configuration.GetSection(WarcraftLogsOptions.SectionName));

// Singleton, damit der OAuth-Token-Cache über alle Polling-Zyklen erhalten bleibt.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WarcraftLogsAuthClient>(sp => new WarcraftLogsAuthClient(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(WarcraftLogsAuthClient)),
    sp.GetRequiredService<IOptions<WarcraftLogsOptions>>()));
builder.Services.AddHttpClient<WarcraftLogsClient>();

builder.Services.AddHostedService<WarcraftLogsPollingService>();

var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey ?? string.Empty)),
        };
    });
builder.Services.AddAuthorization();

const string CorsPolicy = "Frontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<VanguardDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
    await VanillaHistorySeeder.SeedAsync(db);
    await BurningCrusadeHistorySeeder.SeedAsync(db);
    await WrathHistorySeeder.SeedAsync(db);
    await CataclysmHistorySeeder.SeedAsync(db);
    await MistsOfPandariaHistorySeeder.SeedAsync(db);
    await WarlordsHistorySeeder.SeedAsync(db);
    await LegionHistorySeeder.SeedAsync(db);
    await BattleForAzerothHistorySeeder.SeedAsync(db);
    await ShadowlandsHistorySeeder.SeedAsync(db);
    await DragonflightHistorySeeder.SeedAsync(db);
    await TheWarWithinHistorySeeder.SeedAsync(db);
    await MidnightHistorySeeder.SeedAsync(db);
    await GuildProfileSeeder.SeedAsync(db);
    await PvpDemoSeeder.SeedAsync(db);
}

// In Development läuft die API bewusst nur über http://localhost:5000 (siehe
// Properties/launchSettings.json und frontend/.env.example). Ein HTTPS-Redirect
// würde dort den CORS-Preflight und das SignalR-Negotiate des Frontends brechen,
// weil der 307 vor UseCors greift.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RaceHub>("/hubs/race");
app.MapGet("/api/version", () => Results.Ok(new { version = AppInfo.Version, name = "Vanguard API" }));

if (app.Environment.IsDevelopment())
{
    // TEMP (manueller End-to-End-Test der Live-Kill-Updates ohne echte WCL-Credentials):
    // simuliert exakt das Event, das WarcraftLogsPollingService im Live-Betrieb pusht.
    app.MapPost("/api/dev/simulate-kill", async (IHubContext<RaceHub> hub) =>
    {
        var evt = new VanguardTracker.Api.DTOs.LiveTickerEventDto(
            Guid.NewGuid(), "Liquid", "Voidbound Herald",
            "Liquid besiegt Voidbound Herald — Pull #142",
            DateTimeOffset.UtcNow, "kill");
        await hub.Clients.All.SendAsync("TickerEvent", evt);
        await hub.Clients.All.SendAsync("RaceUpdated");
        return Results.Ok(evt);
    });
}

app.Run();
