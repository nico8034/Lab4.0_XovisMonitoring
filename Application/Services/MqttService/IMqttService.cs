namespace Application.Services.MqttService;

public interface IMqttService
{
    void SetupMqttService();
    void StartPublishing();
    void StopPublishing();
    

}