using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace mango.messagebus
{
    public class MessageBus : IMessageBus
    {

        private string Endpoint { get; set; } = "Endpoint=sb://mangowebv2.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=y2gIqVSELYB30lNUhIC/V3tvSukY87mO0+ASbO+plCc=";
        public async Task PublicMessage(object message, string topic_queue_name)
        {
            //use the connection string to create a connection to the Service Bus
            await using (ServiceBusClient client = new ServiceBusClient(Endpoint))
            {
                // create a sender for the queue 
                ServiceBusSender sender = client.CreateSender(topic_queue_name);
                // create a message that we can send
                var jsonMessage = JsonConvert.SerializeObject(message);
                ServiceBusMessage messageBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                };
                await sender.SendMessageAsync(messageBusMessage);
                await client.DisposeAsync();
                
            }

            
                  
        }
    }
}