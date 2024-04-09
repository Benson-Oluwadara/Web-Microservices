using Mango.Services.EmailAPI.Database.IDapperRepositorys;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.DTO;

using Mango.Services.EmailAPI.Services.IServices;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace Mango.Services.EmailAPI.Services.Services
{
    public class EmailService: IEmailService
    {
        private readonly IDapperRepository _dapperRepository;

        public EmailService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task EmailCartAndLog(CartDTO cartdto)
        {
            if (cartdto == null || cartdto.CartHeader == null)
            {
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
            await LogAndEmail(message, email);

            
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

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
