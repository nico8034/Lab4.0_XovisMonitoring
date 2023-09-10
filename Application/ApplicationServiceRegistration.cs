using System.Reflection;
using Application.Services.CameraService;
using Application.Services.ExperimentService;
using Application.Services.ImageProcessingService;
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
        
        services.AddSingleton<ICameraService, XovisCameraService>();
        services.AddSingleton<IExperimentService, ExperimentService>();
        services.AddSingleton<IMonitoringService, MonitoringService>();
        services.AddSingleton<IImageProcessingService, ImageProcessingService>();
        services.AddSingleton<MqttBackgroundService>();
        return services;
    }
}