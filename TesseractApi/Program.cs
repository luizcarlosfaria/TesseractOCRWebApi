using TesseractApi;
using TesseractApi.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TesseractService>();

var app = builder.Build();

app.MapTesseractEndpoints();

app.Run();

