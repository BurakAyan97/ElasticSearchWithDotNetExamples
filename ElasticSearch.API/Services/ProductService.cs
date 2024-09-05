using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repositories;
using System.Collections.Immutable;
using System.Net;

namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var responseProduct = await _productRepository.SaveAsync(request.CreateProduct());

            if (responseProduct is null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "Kayıt esnasında bir hata meydana geldi." }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Success(responseProduct.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            
            var productListDto = new List<ProductDto>();

            foreach (var item in products)
            {
                if (item.Feature is null)
                    productListDto.Add(new ProductDto(item.Id, item.Name, item.Price, item.Stock, null));
                else
                    productListDto.Add(new ProductDto(item.Id, item.Name, item.Price, item.Stock, new ProductFeatureDto(item.Feature.Width, item.Feature.Height, item.Feature.Color)));
            }

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }
    }
}
