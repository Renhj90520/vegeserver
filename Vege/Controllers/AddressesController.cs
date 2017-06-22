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
    public class AddressesController : Controller
    {
        private IVegeRepository vegeRepository;
        public AddressesController(IVegeRepository vegeRepository)
        {
            this.vegeRepository = vegeRepository;
        }
        // GET: api/values
        [HttpGet("{openid?}")]
        public async Task<IActionResult> Get(string openid)
        {
            Result<IEnumerable<Address>> result = new Result<IEnumerable<Address>>();
            try
            {
                result.body = await this.vegeRepository.GetAllAddress(openid);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.body = null;
                result.state = 0;
                result.message = ex.Message;
            }
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Address address)
        {
            Result<Address> result = new Result<Address>();
            try
            {
                var addr = await this.vegeRepository.AddAddress(address);
                if (addr == null)
                {
                    result.state = 0;
                }
                else
                {
                    result.state = 1;
                }
                result.body = addr;
            }
            catch (Exception ex)
            {
                result.body = null;
                result.state = 0;
                result.message = ex.Message;
            }
            return Ok(result);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.DeleteAddr(id))
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
