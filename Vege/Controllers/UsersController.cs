using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;
using Vege.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : Controller
    {
        private IVegeRepository vegeRepository;
        private ILogger<UsersController> log;

        public UsersController(IVegeRepository vegeRepository, ILogger<UsersController> log)
        {
            this.vegeRepository = vegeRepository;
            this.log = log;
        }
        // GET: api/values
        [HttpGet("{type?}")]
        public async Task<IActionResult> GetAllUsers(string type)
        {
            Result<IEnumerable<User>> result = new Result<IEnumerable<User>>();
            try
            {
                result.body = await this.vegeRepository.GetAllUsers(type);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.StackTrace);
            }
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            Result<User> result = new Result<Models.User>();
            try
            {
                user.Password = MD5Encrypter.GetMD5Hash(user.Password);
                var newUser = await this.vegeRepository.AddUser(user);
                result.state = 1;
                result.body = newUser;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.StackTrace);
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
        public void Delete(int id)
        {
        }
    }
}
