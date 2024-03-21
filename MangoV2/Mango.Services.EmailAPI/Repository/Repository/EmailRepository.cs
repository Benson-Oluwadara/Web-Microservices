using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Mango.Services.EmailAPI.Database.IDapperRepositorys;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Repository.IRepository;

namespace Mango.Services.EmailAPI.Repository.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IDapperRepository _dapperRepository;

        public EmailRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        public async Task<int> CreateEmailAsync(EmailLogger email)
        {
            var sql = @"
            INSERT INTO EmailLogger (Email, Message, EmailSent) 
            VALUES (@Email, @Message, @EmailSent);
            SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new
            {
                Email = email.Email,
                Message = email.Message,
                EmailSent = email.EmailSent
            };

            return await _dapperRepository.ExecuteAsync(sql, parameters);
        }

    }
}
