namespace Application.Services.MqttService;

public interface IMqttService
{
    Task SetupMqttService();
    void StartPublishing();
    void StopPublishing();
    Task PublishMessage(string topic, string message);


}