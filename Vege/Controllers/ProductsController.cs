using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Repositories;
using Vege.Models;
using Vege.DTO;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Authorize]
    [Route("api/[controller]/")]
    public class ProductsController : Controller
    {
        private IVegeRepository vegeRepository;
        private IHostingEnvironment env;

        public ProductsController(IVegeRepository vegeRepository, IHostingEnvironment env)
        {
            this.vegeRepository = vegeRepository;
            this.env = env;
        }
        [HttpGet("{id?}")]
        public IActionResult GetAllProducts(int? id, [FromQuery]int? category, [FromQuery]int? index, [FromQuery]int? perPage, [FromQuery] string name)
        {
            Result<ItemsResult<Product>> result = new Result<ItemsResult<Product>>();
            try
            {
                result.body = this.vegeRepository.GetAllProduct(id, category, index, perPage, name);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
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
                    result.state = 0;
                }
                else
                {
                    result.state = 1;
                }
                result.body = newProduct;
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                result.state = 0;
            }

            return Ok(result);
        }

        [HttpDelete("pictures/{picpath}")]
        public async Task<IActionResult> RemovePic(string picpath)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var succ = await this.vegeRepository.RemoveProductPic(picpath);
                if (succ)
                {
                    var fullPath = Path.Combine(this.env.WebRootPath, "upload", picpath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    result.state = 1;
                }
                else
                {
                    result.state = 0;
                }
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody]Product newProduct)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.UpdateProduct(newProduct))
                {
                    result.state = 1;
                }
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
            }
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody]JsonPatchDocument<Product> patchDoc)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.UpdateProduct(id, patchDoc))
                {
                    result.state = 1;
                }
                else
                {
                    result.state = 0;
                }
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
            }
            return Ok(result);
        }
    }
}
