using System.Text.Json.Serialization;
using Serilog;
using Web.Configuration;

var builder = WebApplication.CreateBuilder(args);
var logConfig = new LogConfiguration();
builder.Configuration.GetSection("Logging").Bind(logConfig);

builder.Host.UseSerilog((_, lc) =>
{
    lc.WriteTo.Seq(logConfig.SeqAddress);
    lc.WriteTo.Console();
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.
        Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition = 
        JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSwaggerDocument();

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