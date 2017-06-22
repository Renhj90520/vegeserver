using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;
using Vege.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private IVegeRepository vegeRepository;
        public OrdersController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }

        // GET: api/values
        [HttpGet("{openid?}")]
        public IActionResult GetAllProductsInOrder(string openid, [FromQuery]int? index, [FromQuery]int? perPage, [FromQuery]string keyword, [FromQuery]DateTime? begin, [FromQuery]DateTime? end, [FromQuery]Boolean? noshowRemove)
        {
            Result<ItemsResult<OrderDTO>> result = new Result<ItemsResult<OrderDTO>>();
            try
            {
                result.body = this.vegeRepository.GetAllOrders(openid, index, perPage, keyword, begin, end, noshowRemove);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.body = null;
                result.message = ex.Message;
                result.state = 0;
            }
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Order order)
        {
            Result<Object> result = new Result<object>();
            try
            {
                order.CreateTime = DateTime.Now;
                bool succ = await this.vegeRepository.AddOrder(order);
                if (succ)
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody]JsonPatchDocument<Order> patchDoc)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var succ = await this.vegeRepository.UpdateOrder(id, patchDoc);
                if (succ)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var succ = await this.vegeRepository.RemoveOrder(id);
                if (succ)
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
