using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Services.ModelDTOs;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IOrderingService
    {
        Task<List<Order>> GetMyOrders(ApplicationUser user);
        Task<Order> GetOrder(ApplicationUser user, string orderId);
        void CancelOrder(string orderId);
        Order MapUserInfoIntoOrder(ApplicationUser user, Order order);
        BasketDTO MapOrderToBasket(Order order);
        void OverrideUserInfoIntoOrder(Order original, Order destination);
        Task CreateOrder(ApplicationUser user, int[] itemIds);
    }
}
