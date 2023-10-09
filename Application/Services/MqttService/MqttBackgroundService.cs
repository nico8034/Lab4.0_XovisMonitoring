using Application.Services.MonitoringService;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Application.Services.MqttService;

/// <summary>
/// Background service for handling MQTT Publishing
/// </summary>
public class MqttBackgroundService : IMqttService
{
    private volatile bool isRunning = false;
    private IMqttClient mqttClient;
    private readonly IMonitoringService _monitoringService;

    public MqttBackgroundService(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public bool IsActive()
    {
        return isRunning;
    }

    public IMqttClient GetMqttClient()
    {
        return mqttClient;
    }

    /// <summary>
    /// Setup MQTT - Connect to broker
    /// </summary>
    public MqttClientOptions SetupMqttService()
    {
        // Logic for setting up MQTT
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();
    
        // Targets:
        // digitechi4.tek.sdu.dk:1883
        // tek-sec-cobot-api.tek.sdu.dk:1883
        // 127.0.0.1
        
        
        return new MqttClientOptionsBuilder()
            .WithClientId("XovisZones")
            // .WithTcpServer("localhost", port: 1883)
            // .WithTcpServer("digitechi4.tek.sdu.dk", port: 1883)
            .WithTcpServer("10.126.128.90", port: 1883)
            .WithCleanSession()
            // .WithCredentials("semantic","s3mant1c")
            .Build();
       
        //docker run -it -p 1883:1883 -p 9001:9001 -v C:\Users\nicol\Documents\gitProjects\Lab4.0_XovisMonitoring\mosquitto.conf:/mosquitto/config/mosquitto.conf eclipse-mosquitto
        //docker run -it -p 1883:1883 -p 9001:9001 -v C:\Users\nicol\Desktop\git\Lab4.0_XovisMonitoring\mosquitto.conf:/mosquitto/config/mosquitto.conf eclipse-mosquitto
        
    }

    public async Task Connect()
    {
        var options = SetupMqttService();
        
        //TODO Test getting exception
        await mqttClient.ConnectAsync(options, CancellationToken.None);

        // try
        // {  
        //     // Establish Connection
        //     var connectionResult = await mqttClient.ConnectAsync(options);
        //
        //     if (connectionResult.ResultCode == MqttClientConnectResultCode.Success)
        //     {
        //         Console.WriteLine("Connected to MQTT broker successfully.");
        //     }
        //     else
        //     {
        //         Console.WriteLine($"Failed to connect to MQTT broker: {connectionResult.ResultCode}");
        //     }
        //
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e.Message);
        //     throw;
        // }
    }

    public bool isConnected()
    {
        return mqttClient.IsConnected;
    }

    public void StartPublishing()
    {
        isRunning = true;
        Task.Run(Publishing);
    }

    public async Task StopPublishing()
    {
        isRunning = false;
        await DisconnectFromBroker();
    }
    
    private async Task DisconnectFromBroker()
    {
        if (mqttClient != null && mqttClient.IsConnected)
        {
            try
            {
                await mqttClient.DisconnectAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public async Task PublishMessage(string topic, string message)
    {
        try
        {
            // Logic for publishing a message to a MQTT Topic
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    private async Task Publishing()
    {
        while (isRunning)
        {
            try
            {
                // Logic for how often messages should be published
                string jsonPayload = JsonConvert.SerializeObject(_monitoringService.GetRoom().GetZones());
                await PublishMessage("XovisZones", jsonPayload);
                await Task.Delay(10);
            }
            catch(Exception ex)
            {
                // Log the exception or handle it appropriately.
                // Consider implementing a delay or back-off logic in case of consecutive failures.
                Console.WriteLine(ex.Message);
            }
        }
    }
}