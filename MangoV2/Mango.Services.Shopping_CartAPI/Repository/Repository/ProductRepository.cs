using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Mango.Services.Shopping_CartAPI.Database.IDapperRepositorys;
using Mango.Services.Shopping_CartAPI.Models;
using Mango.Services.Shopping_CartAPI.Models.DTO;
using Mango.Services.Shopping_CartAPI.Repository.IRepository;

namespace Mango.Services.Shopping_CartAPI.Repository.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDapperRepository _dapperRepository;

        public ProductRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var sql = "SELECT * FROM Product";
            return await _dapperRepository.GetAllAsync<ProductDTO>(sql);
        }
    }
        
        
}
