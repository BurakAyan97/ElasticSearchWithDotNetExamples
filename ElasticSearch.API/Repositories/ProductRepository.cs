using ElasticSearch.API.Models;
using Nest;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        private const string indexName = "products";
        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.CreatedTime = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsValid) return null;

            newProduct.Id = response.Id;

            return newProduct;
        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => s.Index(indexName).Query(q => q.MatchAll()));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id; //documents source'dan beslendiği için hit içindeki id'yi ona atıyoruz.

            return result.Documents.ToImmutableList();
        }
    }
}
