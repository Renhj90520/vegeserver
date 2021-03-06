﻿using System;
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
                //result.Body = await this.vegeRepository.GetAllProductInCart(openid);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                result.body = null;
            }

            return Ok(result);
        }

        [HttpPost("{openid?}")]
        public async Task<IActionResult> Post(string openid, [FromBody]CartItem cartItem)
        {
            Result<Object> result = new Result<object>();

            try
            {
                //bool succ = await this.vegeRepository.AddCartItem(openid, cartItem);
                //if (succ)
                //{
                //    result.State = 1;
                //}
                //else
                //{
                //    result.State = 0;
                //}
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
