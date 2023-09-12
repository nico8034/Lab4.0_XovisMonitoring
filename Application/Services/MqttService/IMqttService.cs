using MQTTnet.Client;

namespace Application.Services.MqttService;

public interface IMqttService
{
    MqttClientOptions SetupMqttService();

    IMqttClient GetMqttClient();
    bool isConnected();
    Task Connect();
    void StartPublishing();
    Task StopPublishing();
    Task PublishMessage(string topic, string message);


}