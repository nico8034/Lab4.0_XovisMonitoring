using System.Reflection;
using Application.Services.cameraInfo;
using Application.Services.CameraService;
using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
using Application.Services.Logging;
using Application.Services.MonitoringService;
using Application.Services.MqttService;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();
    // services.Add(AutoMapper(Assembly.GetExecutingAssembly());
    services.AddMediatR(configuration =>
        configuration.RegisterServicesFromAssembly(assembly));

    services.AddValidatorsFromAssembly(assembly);
    services.AddHttpClient();
    services.AddHttpClient<ICameraService, XovisCameraService>(client =>
    {
      client.DefaultRequestHeaders.Authorization =
              new System.Net.Http.Headers.AuthenticationHeaderValue(
                  "Basic",
                  Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:pass"))
              );
      client.Timeout = TimeSpan.FromMilliseconds(150); // FromSeconds(10); // example timeout
    });

    // services.AddSingleton<ICameraService, XovisCameraService>();
    services.AddSingleton<IExperimentService, ExperimentService>();
    services.AddSingleton<IMonitoringService, MonitoringService>();
    services.AddSingleton<IImageProcessingService, ImageProcessingService>();
    services.AddSingleton<IMqttService, MqttBackgroundService>();
    services.AddSingleton<CameraInfoProvider>();
    services.AddSingleton<ILogger, Logger>();

    return services;
  }
}