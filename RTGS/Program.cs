using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using RTGS.Configs;
using RTGS.Services.DB;
using RTGS.Services.Http;
using RTGS.Services.Background;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();

builder.Services.AddSingleton<Config>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<Abs>();
builder.Services.AddHostedService<FileProcessor>();
//builder.Services.AddHostedService<StatusCheck>();
builder.Services.AddDbContext<Db>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseLoggerFactory(NullLoggerFactory.Instance));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

if (app.Environment.IsDevelopment()) app.Run();
else app.Run("http://172.35.11.56:2216");
