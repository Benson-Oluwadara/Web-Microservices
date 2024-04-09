using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Serilog;

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
            try
            {
                var products = await _productService.GetAllProductAsync();
                Log.Information("Returning all products.");
                return Ok(new APIResponse(HttpStatusCode.OK, true, _mapper.Map<IEnumerable<ProductDTO>>(products)));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving all products.");
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while processing your request." }));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    Log.Warning("Product with ID {ProductId} not found.", id);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
                }
                Log.Information("Returning product with ID {ProductId}.", id);

                return Ok(new APIResponse(HttpStatusCode.OK, true, _mapper.Map<ProductDTO>(product)));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving product with ID {ProductId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while processing your request." }));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state while updating product with ID {ProductId}.", id);
                    return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
                }

                var isUpdated = await _productService.UpdateProductAsync(id, updateProductDto);

                if (!isUpdated)
                {
                    Log.Warning("Product with ID {ProductId} not found during update operation.", id);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
                }

                Log.Information("Product with ID {ProductId} updated successfully.", id);
                return Ok(new APIResponse(HttpStatusCode.OK, true, null));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating product with ID {ProductId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while processing your request." }));
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state received while creating a new product.");
                    return BadRequest(new APIResponse(HttpStatusCode.BadRequest, false, null, GetModelStateErrorMessages()));
                }

                var createdProduct = await _productService.CreateProductAsync(createProductDto);

                Log.Information("Product created successfully. Product ID: {ProductId}", createdProduct.ProductId);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, new APIResponse(HttpStatusCode.Created, true, _mapper.Map<ProductDTO>(createdProduct)));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a new product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while processing your request." }));
            }
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
            try
            {
                var isDeleted = await _productService.DeleteProductAsync(id);

                if (!isDeleted)
                {
                    Log.Warning("Product with ID {ProductId} not found during deletion.", id);
                    return NotFound(new APIResponse(HttpStatusCode.NotFound, false, null, new List<string> { "Product not found." }));
                }

                Log.Information("Product with ID {ProductId} deleted successfully.", id);
                return Ok(new APIResponse(HttpStatusCode.OK, true, null));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the product with ID {ProductId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse(HttpStatusCode.InternalServerError, false, null, new List<string> { "An error occurred while processing your request." }));
            }
        }


        private List<string> GetModelStateErrorMessages()
        {
            Log.Warning("Returning model state error messages.");   
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
        }
    }
}
