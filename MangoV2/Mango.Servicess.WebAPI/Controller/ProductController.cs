using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mango.Services.ProductAPI.Controller
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductAsync();
            return Ok(new APIResponse(HttpStatusCode.OK, true, _mapper.Map<IEnumerable<ProductDTO>>(products)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
            }

            return Ok(new APIResponse(HttpStatusCode.OK, true, _mapper.Map<ProductDTO>(product)));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
            }

            var isUpdated = await _productService.UpdateProductAsync(id, updateProductDto);

            if (!isUpdated)
            {
                return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
            }

            return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
            }

            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, new APIResponse(HttpStatusCode.Created, true, _mapper.Map<ProductDTO>(createdProduct)));
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO updateProductDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
        //    }

        //    var isUpdated = await _productService.UpdateProductAsync(id, updateProductDto);

        //    if (!isUpdated)
        //    {
        //        return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
        //    }

        //    return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isDeleted = await _productService.DeleteProductAsync(id);

            if (!isDeleted)
            {
                return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
            }

            return Ok(new APIResponse(HttpStatusCode.OK, true, null));
        }

        private List<string> GetModelStateErrorMessages()
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
        }
    }
}
