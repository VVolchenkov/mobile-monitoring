using System.Text.Json.Serialization;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Migrations.Extensions;
using Mapster;
using MapsterMapper;
using Serilog;
using Serilog.Core;
using Web;
using Web.Controllers;
using Web.Hubs;
using Web.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationRoot? configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
    .AddEnvironmentVariables()
    .Build();

var rabbitMqConfiguration = new RabbitMqConfiguration();
configuration.Bind("RabbitMq", rabbitMqConfiguration);

Logger? logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog(logger);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSwaggerDocument();
builder.Services.AddInfrastructure(configuration, rabbitMqConfiguration);

var config = new TypeAdapterConfig();
config.Apply(new MappingRegister());
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddScoped<DeviceHub>();
builder.Services.AddSignalR();
builder.Services.AddCors(x => x.AddPolicy("CorsPolicy", corsPolicyBuilder =>
{
    corsPolicyBuilder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
}));

builder.Services.AddScoped<IDeviceService, DeviceService>();

WebApplication app = builder.Build();

IServiceProvider serviceProvider = app.Services;
var migrationManager = serviceProvider.GetRequiredService<MigrationManager>();
migrationManager.MigrateDatabase(serviceProvider, configuration);

app.UseCors("CorsPolicy");
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseOpenApi();
app.UseSwaggerUi3();

app.MapHub<DeviceHub>("/deviceHub");

app.Run();
