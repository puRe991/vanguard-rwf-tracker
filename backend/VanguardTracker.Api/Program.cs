using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.Hubs;
using VanguardTracker.Api.Services;
using VanguardTracker.Api.WarcraftLogs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RaceHub>("/hubs/race");

app.Run();
