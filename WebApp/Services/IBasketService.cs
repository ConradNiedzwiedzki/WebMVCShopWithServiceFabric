using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IBasketService
    {
        Task<Basket> GetBasket(ApplicationUser user, HttpRequest request);
    }
}
