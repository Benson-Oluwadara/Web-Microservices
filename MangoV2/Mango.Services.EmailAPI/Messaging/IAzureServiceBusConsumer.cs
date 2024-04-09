using Mango.Services.EmailAPI.Services.IServices;

namespace Mango.Services.EmailAPI.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        //IEmailService EmailService { get; set; } // Define the EmailService property

        Task Start();
        Task Stop();
    }
}
