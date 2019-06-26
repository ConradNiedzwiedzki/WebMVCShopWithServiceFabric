using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using WebMVC.Services;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IBasketService _basketSvc;
        private readonly ICatalogService _catalogSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        public CartController(IBasketService basketSvc, ICatalogService catalogSvc, IIdentityParser<ApplicationUser> appUserParser)
        {
            _basketSvc = basketSvc;
            _catalogSvc = catalogSvc;
            _appUserParser = appUserParser;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var vm = await _basketSvc.GetBasket(user, Request);

                return View(vm);
            }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
            }

            return View();
        }

        public IActionResult AddToCart(CatalogItem productDetails)
        {
            try
            {
                if (productDetails?.Id != null)
                {
                    var cookieBasket = Request.Cookies["basket"];
                    if (cookieBasket != null)
                    {
                        var backetIds = cookieBasket.Split(',').Select(int.Parse).ToList();
                        if (!backetIds.Contains(productDetails.Id))
                        {
                            Response.Cookies.Append("basket", cookieBasket + "," + productDetails.Id);
                        }
                    }
                    else
                    {
                        Response.Cookies.Append("basket", productDetails?.Id.ToString());
                    }
                }

                return RedirectToAction("Index", "Catalog");
            }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
            }

            return RedirectToAction("Index", "Catalog", new { errorMsg = ViewBag.BasketInoperativeMsg });
        }

        private void HandleBrokenCircuitException()
        {
            ViewBag.BasketInoperativeMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
        }

        [HttpPost]
        public IActionResult Detail(int id)
        {
            BasketItem bi = BasketHelper.GetBusketItem(id);
            return View(bi);
        }
    }
}
