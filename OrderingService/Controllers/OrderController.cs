using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace OrderingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static readonly List<JObject> Orders = new List<JObject>();

        [HttpGet]
        public ActionResult<JObject[]> GetOrders()
        {
            return Orders.ToArray();
        }

        [HttpPost]
        public void PostOrder([FromBody]JObject item)
        {
            Orders.Add(item);
        }
    }
}
