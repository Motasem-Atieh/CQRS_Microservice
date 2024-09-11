//namespace CQRS_Microservice.Messaging
//{

public interface IRabbitMQService
    {

    public void SendingMessage<T>(T message);
    }
//}
