namespace Application.Mqtt.Exceptions;

public class MqttClientNotInstantiated : Exception
{
    public MqttClientNotInstantiated() :base("Mqtt client is not instantiated"){}
  
}