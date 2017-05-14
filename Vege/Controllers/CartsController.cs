using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.DTO;
using Vege.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class CartsController : Controller
    {
        private IVegeRepository vegeRepository;
        public CartsController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }
        // GET: api/values
        [HttpGet("{openid?}")]
        public async Task<IActionResult> GetAllCartProducts(string openid)
        {
            Result<IEnumerable<CartItemDTO>> result = new Result<IEnumerable<CartItemDTO>>();
            try
            {
                result.Body = await this.vegeRepository.GetAllProductInCart(openid);
                result.State = 1;
            }
            catch (Exception ex)
            {
                result.State = 0;
                result.Message = ex.Message;
                result.Body = null;
            }

            return Ok(result);
        }

        [HttpPost("{openid?}")]
        public void Post(string openid, [FromBody]int id, [FromBody]int count)
        {
            
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
