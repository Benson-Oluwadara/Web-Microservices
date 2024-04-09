using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Database.IDapperRepositorys;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.DTO;
using Mango.Services.EmailAPI.Services.IServices;
using Mango.Services.EmailAPI.Services.Services;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;//
        private readonly string emailCartQueue; //
        private readonly IConfiguration _configuration;//
        //private  IEmailService _emailService;
        private ServiceBusProcessor _emailCartProcessor;//
        private readonly IDapperRepository _dapperRepository;//
        
        //private ServiceBusProcessor _emailOrderPlacedProcessor;
        private readonly string registerUserQueue;
        private ServiceBusProcessor _registerUserProcessor;
        //private readonly string orderCreated_Topic;
        //private readonly string orderCreated_Email_Subscription;
        //private ServiceBusProcessor _emailCartProcessor;
        //private readonly ILogger<AzureServiceBusConsumer> _logger; // Add ILogger dependency

        public AzureServiceBusConsumer(IConfiguration configuration, IDapperRepository dapperRepository)
        {

            _configuration = configuration;//
            _dapperRepository = dapperRepository;//
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");//
            //print the value of the serviceBusConnectionString
            Console.WriteLine("serviceBusConnectionString: " + serviceBusConnectionString);
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");//
            //get the value of the emailCartQueue
            Console.WriteLine("emailCartQueue: " + emailCartQueue);
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
            //orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            //orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);//
            //get the value of the client
            Console.WriteLine("client: " + client);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);//
            //get the value of the emailCartProcessor
            Console.WriteLine("emailCartProcessor: " + _emailCartProcessor);
            //_emailService = emailService;
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            //_emailOrderPlacedProcessor = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);

        }
        
       
        public async Task Start()//
        {

            // Make sure _emailService is not null before proceeding
            
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;//
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;//
            
            await _emailCartProcessor.StartProcessingAsync();//
            Log.Information("Starting Email Cart processor...");
            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();
            Log.Information("Email Cart processor started.");

            //_emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            //_emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            //await _emailOrderPlacedProcessor.StartProcessingAsync();

        }
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            try { 
            //print the args from parameters
            Console.WriteLine("args: " + args);
            //this is where you will receive message

            //get the message from the args
            Console.WriteLine("args.Message: " + args.Message);

            var message = args.Message;
            //get the body of the message
            Console.WriteLine("message.Body: " + message.Body);
            //get the body of the message and convert it to string


            var body = Encoding.UTF8.GetString(message.Body);
            //get the body of the message and convert it to string
            Console.WriteLine("body: " + body);

            CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);
                // Logging structured information
                Log.Information("Received email cart request: {@CartDTO}", objMessage);

                //get the objMessage
                Console.WriteLine("objMessage: " + objMessage);

                //TODO - try to log email
                await EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
                Log.Information("Email cart request processed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing email cart request");
                throw;
            }

        }
        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            //print the args from parameters
            Console.WriteLine("args: " + args);
            //this is where you will receive message

            //get the message from the args
            Console.WriteLine("args.Message: " + args.Message);
            //this is where you will receive message
            var message = args.Message;
            //get the body of the message
            Console.WriteLine("message.Body: " + message.Body);
            //get the body of the message and convert it to string
            var body = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine("body: " + body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //TODO - try to log email
                //await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        
        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            //print the args from parameters
            Console.WriteLine("args: " + args);
            //this is where you will receive message

            //get the message from the args
            Console.WriteLine("args.Message: " + args.Message);
            //this is where you will receive message
            var message = args.Message;
            
            //get the body of the message
            Console.WriteLine("message.Body: " + message.Body);
            var body = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine("body: " + body);
            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TODO - try to log email
                await RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing email cart request: " + ex.Message);
                throw;
            }
        }
        public async Task Stop()//
        {
            Log.Information("Stopping Email Cart processor...");

            await _emailCartProcessor.StopProcessingAsync();//
            await _emailCartProcessor.DisposeAsync();//

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
            Log.Information("Email Cart processor stopped.");
            //await _emailOrderPlacedProcessor.StopProcessingAsync();
            //await _emailOrderPlacedProcessor.DisposeAsync();

        }

        public async Task EmailCartAndLog(CartDTO cartdto)
        {
            if (cartdto == null || cartdto.CartHeader == null)
            {
                Log.Warning("Received invalid cart DTO: {@CartDTO}", cartdto);
                // Handle null cartdto or cart header appropriately
                return;
            }

            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartdto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartdto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");
            await LogAndEmail(message.ToString(), cartdto.CartHeader.Email);
        }



        public async Task LogOrderPlaced(RewardsMessage rewardsDTO)
        {
            string message = "New Order Placed. <br/> Order ID : " + rewardsDTO.OrderId;
            await LogAndEmail(message, "benson.oluwadara@gmail.com");

            // Return a default value of type T or another meaningful result

        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registration Successful. <br/> Email: " + email;
            await LogAndEmail(message,"benson.oluwadara@gmail.com");


        }


        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                var sql = @"
            INSERT INTO EmailLogger (Email, Message, EmailSent) 
            VALUES (@Email, @Message, @EmailSent);
            SELECT CAST(SCOPE_IDENTITY() as int)";

                await _dapperRepository.ExecuteAsync(sql, emailLog);
                Log.Information("Email logged and sent successfully to {Email}", email);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error logging or sending email to {Email}", email);

                return false;
            }
        }
    }


}
