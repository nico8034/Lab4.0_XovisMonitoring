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

    public bool IsActive()
    {
        return isRunning;
    }

    /// <summary>
    /// Setup MQTT - Connect to broker
    /// </summary>
    public async Task SetupMqttService()
    {
        // Logic for setting up MQTT
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId("XovisZones")
            .WithTcpServer("127.0.0.1", port: 1883)
            .WithCleanSession()
            .Build();

        try
        {
            await mqttClient.ConnectAsync(options);
            Console.WriteLine("MQTT Connected");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
            
        // TODO: Add reconnect policies and other necessary configurations.
    }

    public void StartPublishing()
    {
        isRunning = true;
        Task.Run(Publishing);
    }

    public void StopPublishing()
    {
        isRunning = false;
        DisconnectFromBroker();
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
                // Will use PublishMessage Method
                await PublishMessage("yourTopic", "yourMessage"); // Adjust these values as per your logic.
                await Task.Delay(2000);
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