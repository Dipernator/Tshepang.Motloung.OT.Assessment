using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OT.Assessment.Database;
using OT.Assessment.Models;
using OT.Assessment.Service;
using OT.Assessment.Service.Interface;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        //configure services
        services.Configure<MessageQueuingSettings>(context.Configuration.GetSection("MessageQueuing"));
        services.AddScoped<IRabbitMQService, RabbitMQService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(context.Configuration.GetConnectionString("DatabaseConnection")));
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

// Get the service and start consuming
var rabbitMQService = host.Services.GetRequiredService<IRabbitMQService>();
await rabbitMQService.ReadCasinoWagerAsync();

await host.RunAsync();

logger.LogInformation("Application ended {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);