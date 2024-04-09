using Mango.Services.OrderAPI.Database.IDapperRepositorys;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Service.IService;

namespace Mango.Services.OrderAPI.Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IDapperRepository _dapperRepository;
        public OrderService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        //public async Task<IEnumerable<OrderDetails>> GetOrderDetailsAsync()
        //{
        //    string sql = "SELECT * FROM OrderDetails";
        //    return await _dapperRepository.GetAllAsync<OrderDetails>(sql);
        //}
        public async Task<IEnumerable<OrderDetails>> GetAllOrdersAsync()
        {
            string sql = @"
            SELECT od.*
            FROM OrderDetails od
            INNER JOIN OrderHeaders oh ON od.OrderHeaderId = oh.OrderHeaderId
            ORDER BY oh.OrderHeaderId DESC;
        ";
            return await _dapperRepository.GetAllAsync<OrderDetails>(sql);
        }
        public async Task<IEnumerable<OrderHeader>> GetUserOrdersAsync(string userId)
        {
            string sql = @"
        SELECT oh.*
        FROM OrderHeaders oh
        INNER JOIN OrderDetails od ON oh.OrderHeaderId = od.OrderHeaderId
        WHERE oh.UserId = @UserId
        ORDER BY oh.OrderHeaderId DESC;
    ";
            return await _dapperRepository.GetAllAsync<OrderHeader>(sql, new { UserId = userId });
        }

        public async Task<OrderHeader> GetOrderByIdAsync(int orderId)
        {
            string sql = "SELECT * FROM OrderHeaders WHERE OrderHeaderId = @OrderId";
            var parameters = new { OrderId = orderId };
            return await _dapperRepository.GetAsync<OrderHeader>(sql, parameters);
        }

        public async Task<int> InsertOrderHeaderAsync(OrderHeader orderHeader)
        {
            // Define the SQL insert query
            string sql = @"
                INSERT INTO OrderHeaders (UserId, CouponCode, Discount, OrderTotal, Name, Phone, Email, OrderTime, Status, PaymentIntentId, StripeSessionId)
                VALUES (@UserId, @CouponCode, @Discount, @OrderTotal, @Name, @Phone, @Email, @OrderTime, @Status, @PaymentIntentId, @StripeSessionId);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            // Execute the insert query and return the inserted OrderHeaderId
            return await _dapperRepository.GetAsync<int>(sql, orderHeader);
        }

    }
}
