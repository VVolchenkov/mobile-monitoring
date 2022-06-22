using System.Diagnostics;
using System.Text.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lc) =>
{
    lc.WriteTo.File("log.txt");
    if (Debugger.IsAttached)
    {
        lc.WriteTo.Console();
    }
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.
        Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition = 
        JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();