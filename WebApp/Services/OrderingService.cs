using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebMVC.Infrastructure;
using WebMVC.Services.ModelDTOs;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public class OrderingService : IOrderingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteUrl;
        private static int _orderIndex;


        public OrderingService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _remoteUrl = $"{settings.Value.OrderingService}/api/order";
        }

        public async Task<Order> GetOrder(ApplicationUser user, string id)
        {
            var orders = await GetMyOrders(user);
            return orders.SingleOrDefault(x => x.OrderNumber == id);
        }

        public async Task<List<Order>> GetMyOrders(ApplicationUser user)
        {
            var uri = API.Order.GetOrders(_remoteUrl);
            var responseString = await _httpClient.GetStringAsync(uri);
            var response = JsonConvert.DeserializeObject<List<Order>>(responseString);
            return await Task.Factory.StartNew(() => response);
        }

        public void CancelOrder(string orderId)
        {
        }

        public async Task CreateOrder(ApplicationUser user, int[] itemIds)
        {
            var order = new Order
            {
                OrderNumber = _orderIndex++.ToString(),
                Date = DateTime.Now
            };
            foreach (var itemId in itemIds)
            {
                var orderItem = new OrderItem {ProductId = itemId};
                order.OrderItems.Add(orderItem);
            }

            var data = JsonConvert.SerializeObject(order);
            var uri = API.Order.PostOrder(_remoteUrl);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Cannot create order");
            }
        }

        public void OverrideUserInfoIntoOrder(Order original, Order destination)
        {
            destination.City = original.City;
            destination.Street = original.Street;
            destination.State = original.State;
            destination.Country = original.Country;
            destination.ZipCode = original.ZipCode;

            destination.CardNumber = original.CardNumber;
            destination.CardHolderName = original.CardHolderName;
            destination.CardExpiration = original.CardExpiration;
            destination.CardSecurityNumber = original.CardSecurityNumber;
        }

        public Order MapUserInfoIntoOrder(ApplicationUser user, Order order)
        {
            order.City = user.City;
            order.Street = user.Street;
            order.State = user.State;
            order.Country = user.Country;
            order.ZipCode = user.ZipCode;

            order.CardNumber = user.CardNumber;
            order.CardHolderName = user.CardHolderName;
            order.CardExpiration = new DateTime(int.Parse("20" + user.Expiration.Split('/')[1]), int.Parse(user.Expiration.Split('/')[0]), 1);
            order.CardSecurityNumber = user.SecurityNumber;

            return order;
        }

        public BasketDTO MapOrderToBasket(Order order)
        {
            order.CardExpirationApiFormat();

            return new BasketDTO
            {
                City = order.City,
                Street = order.Street,
                State = order.State,
                Country = order.Country,
                ZipCode = order.ZipCode,
                CardNumber = order.CardNumber,
                CardHolderName = order.CardHolderName,
                CardExpiration = order.CardExpiration,
                CardSecurityNumber = order.CardSecurityNumber,
                CardTypeId = 1,
                Buyer = order.Buyer,
                RequestId = order.RequestId
            };
        }
    }
}
