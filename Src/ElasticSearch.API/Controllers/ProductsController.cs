using ElasticSearch.API.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ElasticClient _elasticClient;

        public ProductsController(ILogger<ProductsController> logger
            , ElasticClient elasticClient)
        {
            _logger = logger;
            _elasticClient = elasticClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>();

            return Ok(searchResponse.Documents ?? default);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var getResponse = await _elasticClient.GetAsync<Product>(id);

            return Ok(getResponse.Source ?? default);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Product product)
        {
            if (product.Id <= 0)
                product.Id = new Random().Next(int.MaxValue);

            product.CreatedDate = DateTime.Now;

            string indexName = "products";
            await ChekIndex(indexName);

            var response = await _elasticClient.CreateAsync(product, q => q.Index(indexName));
            if (response.ApiCall?.HttpStatusCode == 409)
            {
                await _elasticClient.UpdateAsync<Product>(response.Id, a => a.Index(indexName).Doc(product));
            }

            return Ok(product);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task ChekIndex(string indexName)
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName);
            if (existsResponse.Exists)
                return;

            await _elasticClient.Indices.CreateAsync(indexName, ci => ci.Index(indexName));
        }
    }
}
