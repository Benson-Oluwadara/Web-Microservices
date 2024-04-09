using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        //Task<IEnumerable<OrderHeader>> GetOrderHeadersAsync();
        Task<IEnumerable<OrderDetails>> GetAllOrdersAsync();
        Task<IEnumerable<OrderHeader>> GetUserOrdersAsync(string userId);
        //Task<IEnumerable<OrderDetails>> GetOrderDetailsAsync();
        Task<OrderHeader> GetOrderByIdAsync(int orderId);
        Task<int> InsertOrderHeaderAsync(OrderHeader orderHeader);


    }
}
