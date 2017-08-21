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
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]/")]
    public class ProductsController : Controller
    {
        private IVegeRepository vegeRepository;
        private IHostingEnvironment env;
        private ILogger<ProductsController> log;

        public ProductsController(IVegeRepository vegeRepository, IHostingEnvironment env, ILogger<ProductsController> log)
        {
            this.vegeRepository = vegeRepository;
            this.env = env;
            this.log = log;
        }
        [HttpGet]
        public IActionResult GetAllProducts([FromQuery]int? category, [FromQuery]int? index, [FromQuery]int? perPage, [FromQuery] string name, [FromQuery] int? state)
        {
            Result<ItemsResult<ProductDTO>> result = new Result<ItemsResult<ProductDTO>>();
            try
            {
                result.body = this.vegeRepository.GetAllProduct(category, index, perPage, name, state);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            Result<Product> result = new Result<Product>();
            try
            {
                result.body = await vegeRepository.GetProduct(id);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [Authorize]
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
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return Ok(result);
        }

        [Authorize]
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
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [Authorize]
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
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [Authorize]
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
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPatch("exchange/{id1}/{id2}")]
        public async Task<IActionResult> ReOrder(int id1, int id2)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var succ = await this.vegeRepository.reorder(id1, id2);
                if (succ)
                {
                    result.state = 1;
                    result.body = succ;
                }
                else
                {
                    result.state = 0;
                    result.message = "商品不存在";
                    log.LogDebug("排序返回false 商品 id:" + id1 + "," + id2);
                }
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }
    }
}
