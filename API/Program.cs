using System.Reflection;
using API;
using Application;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger documentation configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(name:"v1",new OpenApiInfo(){Title = "Xovis Camera API", Version = "v1"});
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services
    .AddApplicationServices();

builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();