namespace WebMVC.Infrastructure
{
    public static class API
    {
        public static class Order
        {
            public static string GetOrders(string baseUri)
            {
                return $"{baseUri}";
            }

            public static string GetOrder(string baseUri, string orderId)
            {
                return $"{baseUri}/{orderId}";
            }

            public static string PostOrder(string baseUri)
            {
                return $"{baseUri}";
            }

            public static string ShipOrder(string baseUri)
            {
                return $"{baseUri}/ship";
            }
        }

        public static class Catalog
        {
            public static string GetStock(string baseUri, int id)
            {
                return $"{baseUri}/{id}";
            }
        }
    }
}