using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private IVegeRepository vegeRepository;
        public OrderController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }

        // GET: api/values
        [HttpGet("/{openid?}")]
        public IActionResult GetAllProductsInOrder(string openid,[FromQuery]int? index,[FromQuery]int? perPage)
        {
            Result<IEnumerable<Order>> result = new Result<IEnumerable<Order>>();
            try
            {
                result.Body = this.vegeRepository.GetAllOrders(openid,index,perPage);
                result.State = 1;
            }
            catch (Exception ex)
            {
                result.Body = null;
                result.Message = ex.Message;
                result.State = 0;
            }
            return Ok(result);
        }

        // POST api/values
        [HttpPost("/{openid?}")]
        public async Task<IActionResult> Post(string opendid, [FromBody]Order order)
        {
            Result<Object> result = new Result<object>();
            try
            {
                bool succ = await this.vegeRepository.AddOrder(order);
                if (succ)
                {
                    result.State = 1;
                }
                else
                {
                    result.State = 0;
                }
            }
            catch (Exception ex)
            {
                result.State = 0;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
    }
}
