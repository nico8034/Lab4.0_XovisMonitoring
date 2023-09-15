namespace Application.Mqtt.Exceptions;

public class MqttNotConnected : Exception
{
    public MqttNotConnected() :base("Mqtt client is not connected"){}
  
}