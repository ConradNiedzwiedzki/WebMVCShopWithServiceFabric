using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly ILogger<BasketService> _logger;
        private ICatalogService _catalogSvc;

        public BasketService(HttpClient httpClient, ILogger<BasketService> logger, IOptions<AppSettings> settings, ICatalogService catalogSvc)
        {
            _settings = settings;
            _logger = logger;
            _catalogSvc = catalogSvc;
        }

        public async Task<Basket> GetBasket(ApplicationUser user, HttpRequest request)
        {
            var cookieBasket = request.Cookies["basket"];
            var basket = new Basket();
            basket.Items = new List<BasketItem>();

            if (cookieBasket != null)
            {
                var backetIds = cookieBasket.Split(',').Select(int.Parse).ToList();
                foreach (var item in backetIds)
                {
                    var busketItem = BasketHelper.GetBusketItem(item);
                    busketItem.QuantityStock = await _catalogSvc.CheckStockAvailability(item);
                    basket.Items.Add(busketItem);
                }
            }

            return basket;
        }
    }

    public static class BasketHelper
    {
        public static BasketItem GetBusketItem(int id)
        {
            switch (id)
            {
                case 1:
                    return new BasketItem()
                    {
                        Id = "1",
                        OldUnitPrice = 19.5m,
                        PictureUrl = "1.png",
                        ProductId = "1",
                        ProductName = ".NET Bot Black Hoodie",
                        Quantity = 1,
                        UnitPrice = 19.5m
                    };
                case 2:
                    return new BasketItem()
                    {
                        Id = "2",
                        OldUnitPrice = 19.5m,
                        PictureUrl = "2.png",
                        ProductId = "2",
                        ProductName = ".NET Black & White Mug",
                        Quantity = 1,
                        UnitPrice = 19.5m
                    };
                case 3:
                    return new BasketItem()
                    {
                        Id = "3",
                        OldUnitPrice = 29.5m,
                        PictureUrl = "3.png",
                        ProductId = "3",
                        ProductName = "Prism White T-Shirt",
                        Quantity = 1,
                        UnitPrice = 29.5m
                    };
                default:
                    return new BasketItem()
                    {
                        Id = "1",
                        OldUnitPrice = 19.5m,
                        PictureUrl = "1.png",
                        ProductId = "1",
                        ProductName = ".NET Bot Black Hoodie",
                        Quantity = 1,
                        UnitPrice = 19.5m
                    };
            }
        }
    }
}
