using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebMVC.Infrastructure;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly ILogger<CatalogService> _logger;
        private HttpClient _httpClient;
        private readonly string _remoteCatalogBaseUrl;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _logger = logger;
            _remoteCatalogBaseUrl = $"{settings.Value.CatalogService}/api/stock";
        }

        public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
        {
            var catalog = new Catalog();
            catalog.Data = new List<CatalogItem>();
            catalog.Count = 3;
            

            catalog.Data.Add(new CatalogItem()
            {
                CatalogBrand = ".Net",
                CatalogBrandId = 1,
                CatalogType = "T-Shirt",
                CatalogTypeId =1 ,
                Id = 1,
                Description = ".NET Bot Black Hoodie, and more",
                Name = ".NET Bot Black Hoodie",
                PictureUri = "1.png",
                Price = 19.5m
            });

            catalog.Data.Add(new CatalogItem()
            {
                CatalogBrand = ".Net",
                CatalogBrandId = 1,
                CatalogType = "T-Shirt",
                CatalogTypeId = 1,
                Id = 2,
                Description = ".NET Black & White Mug",
                Name = ".NET Black & White Mug",
                PictureUri = "2.png",
                Price = 49.5m
            });

            catalog.Data.Add(new CatalogItem()
            {
                CatalogBrand = ".Net",
                CatalogBrandId = 1,
                CatalogType = "T-Shirt",
                CatalogTypeId = 1,
                Id = 3,
                Description = "Prism White T-Shirt",
                Name = "Prism White T-Shirt",
                PictureUri = "3.png",
                Price = 29.5m
            });

            return await Task.Factory.StartNew(() => catalog);
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var items = new List<SelectListItem>();

            items.Add(new SelectListItem()
            {
                Value = "1",
                Text = ".NET"
            });

            return await Task.Factory.StartNew(() => items);
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var items = new List<SelectListItem>();

            items.Add(new SelectListItem()
            {
                Value = "1",
                Text = "T-Shirt"
            });

            return await Task.Factory.StartNew(() => items);

        }

        public async Task<int> CheckStockAvailability(int id)
        {
            var uri = API.Catalog.GetStock(_remoteCatalogBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(uri);

            return int.Parse(responseString);
        }

        public async Task RemoveItemFromStock(int id)
        {
            var uri = API.Catalog.GetStock(_remoteCatalogBaseUrl, id);

            await _httpClient.DeleteAsync(uri);
        }
    }
}
