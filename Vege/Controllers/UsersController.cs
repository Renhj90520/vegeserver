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
using Vege.DTO;

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
        [HttpGet()]
        public async Task<IActionResult> GetAllUsers([FromQuery]int? index, [FromQuery]int perPage, [FromQuery]string keyword)
        {
            Result<ItemsResult<User>> result = new Result<ItemsResult<User>>();
            try
            {
                result.body = await this.vegeRepository.GetAllUsers(index, perPage, keyword);
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
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        // PUT api/values/5
        [HttpPut("changepwd")]
        public async Task<IActionResult> ChangePwd([FromBody]PwdWrapper wrapper)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var re = await this.vegeRepository.changePwd(wrapper.UserName, wrapper.OldPwd, wrapper.NewPwd, result);
                if (re)
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

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
