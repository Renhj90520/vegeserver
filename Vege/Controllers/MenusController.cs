using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Repositories;
using Microsoft.Extensions.Logging;
using Vege.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Vege.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class MenusController : Controller
    {
        private IVegeRepository vegeRepository;
        private ILogger<MenusController> log;
        private IHostingEnvironment env;

        public MenusController(IVegeRepository vegeRepository, ILogger<MenusController> log, IHostingEnvironment env)
        {
            this.vegeRepository = vegeRepository;
            this.log = log;
            this.env = env;
        }
        // GET: api/values
        [HttpGet("menulist")]
        public async Task<IActionResult> getMenus([FromQuery]int? index, [FromQuery]int? perPage, [FromQuery]string state)
        {
            var result = new Result<ItemsResult<Menu>>();
            try
            {
                result.body = await this.vegeRepository.getAllMenus(index, perPage, state);
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
        public async Task<IActionResult> Post([FromBody]Menu menu)
        {
            var result = new Result<bool>();
            try
            {
                await this.vegeRepository.addMenu(menu);
                result.state = 1;
                result.body = true;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveMenu(int id)
        {
            var result = new Result<bool>();
            try
            {
                if (await vegeRepository.delMenu(id))
                {
                    result.state = 1;
                }
                else
                {
                    result.state = 0;
                    result.message = "删除失败";
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(int id)
        {
            var result = new Result<MenuDTO>();
            try
            {
                result.body = await vegeRepository.getMenu(id);
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

        [HttpDelete("{id}/pictures/{picPath}")]
        public async Task<IActionResult> RemovePic(int id, string picPath)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var succ = await this.vegeRepository.RemoveMenuPic(id);
                if (succ)
                {
                    var fullPath = Path.Combine(this.env.WebRootPath, "upload", picPath);
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
    }
}
