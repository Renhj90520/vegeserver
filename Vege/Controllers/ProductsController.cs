using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Repositories;
using Vege.Models;
using Vege.DTO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]/")]
    public class ProductsController : Controller
    {
        private IVegeRepository vegeRepository;
        public ProductsController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }
        [HttpGet("{id?}")]
        public IActionResult GetAllProducts(int? id, [FromQuery]int? category, [FromQuery]int? index, [FromQuery]int? perPage)
        {
            Result<ItemsResult<Product>> result = new Result<ItemsResult<Product>>();
            try
            {
                result.Body = this.vegeRepository.GetAllProduct(id, category, index, perPage);
                result.State = 1;
            }
            catch (Exception ex)
            {
                result.State = 0;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
        [HttpPost("")]
        public async Task<IActionResult> AddProduct([FromBody]Product product)
        {
            Result<Product> result = new Result<Product>();

            try
            {
                var newProduct = await this.vegeRepository.AddProduct(product);
                if (newProduct == null)
                {
                    result.State = 0;
                }
                else
                {
                    result.State = 1;
                }
                result.Body = newProduct;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = 0;
            }

            return Ok(result);
        }
    }
}
