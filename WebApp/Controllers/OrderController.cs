using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Services;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderingService _orderSvc;
        private IBasketService _basketSvc;
        private readonly ICatalogService _catalogSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        public OrderController(IOrderingService orderSvc, IBasketService basketSvc, IIdentityParser<ApplicationUser> appUserParser, ICatalogService catalogSvc)
        {
            _appUserParser = appUserParser;
            _orderSvc = orderSvc;
            _basketSvc = basketSvc;
            _catalogSvc = catalogSvc;
        }

        public IActionResult Create()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var vm = _orderSvc.MapUserInfoIntoOrder(user, new Order());
            vm.CardExpirationShortFormat();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var cookieBasket = Request.Cookies["basket"];
            if (cookieBasket != null)
            {
                var backetIds = cookieBasket.Split(',').Select(int.Parse).ToList();

                backetIds.ForEach(_id => _catalogSvc.RemoveItemFromStock(_id));
                await _orderSvc.CreateOrder(user, backetIds.ToArray());

                Response.Cookies.Delete("basket");
            }

            return RedirectToAction("Index");
        }


        public IActionResult Cancel(string orderId)
        {
            _orderSvc.CancelOrder(orderId);

            //Redirect to historic list.
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(string orderId)
        {
            var user = _appUserParser.Parse(HttpContext.User);

            var order = await _orderSvc.GetOrder(user, orderId);
            return View(order);
        }

        public async Task<IActionResult> Index(Order item)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var vm = await _orderSvc.GetMyOrders(user);
            return View(vm);
        }
    }
}