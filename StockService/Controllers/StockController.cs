using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace StockService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        //item id, quantity
        private static readonly Dictionary<int, int> Stock = new Dictionary<int, int>();

        static StockController()
        {
            Stock.Add(1, 9);
            Stock.Add(2, 4);
            Stock.Add(3, 15);
        }

        [HttpGet]
        [Route("{itemId}")]
        public ActionResult<int> CheckStockAvailability(int itemId)
        {
            return Stock.FirstOrDefault(p => p.Key == itemId).Value;
        }

        [HttpDelete]
        [Route("{itemId}")]
        public void RemoveItemFromStock(int itemId)
        {
            Stock[itemId] = Stock[itemId] - 1;
        }
    }
}
