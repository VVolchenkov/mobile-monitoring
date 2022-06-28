using System.Text.Json.Serialization;
using Infrastructure;
using Mapster;
using MapsterMapper;
using Serilog;
using Web;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
    .AddEnvironmentVariables()
    .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog(logger);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSwaggerDocument();
builder.Services.AddInfrastructure(configuration.GetConnectionString("PostgreSql"));

var config = new TypeAdapterConfig();
config.Apply(new MappingRegister());
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();



var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseOpenApi();
app.UseSwaggerUi3();

app.Run();