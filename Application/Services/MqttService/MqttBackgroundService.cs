using Application.Services.MonitoringService;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;

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

        return new MqttClientOptionsBuilder()
            .WithClientId("XovisZones")
            .WithTcpServer("127.0.0.1", port: 1883)
            .WithCleanSession()
            .Build();
       
        //docker run -it -p 1883:1883 -p 9001:9001 -v C:\Users\nicol\Documents\gitProjects\Lab4.0_XovisMonitoring\mosquitto.conf:/mosquitto/config/mosquitto.conf eclipse-mosquitto
    }

    public async Task Connect()
    {
        var options = SetupMqttService();
        
        try
        {  
            // Establish Connection
            var connectionResult = await mqttClient.ConnectAsync(options);

            if (connectionResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {connectionResult.ResultCode}");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
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
                await PublishMessage("XovisZones", $"{_monitoringService.GetRoom().GetZones().Values.ToList()}");
                await Task.Delay(100);
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