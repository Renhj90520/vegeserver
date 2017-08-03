using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private IVegeRepository vegeRepository;
        private IHostingEnvironment env;
        private ILogger<CategoriesController> log;

        public CategoriesController(IVegeRepository vegeRepository, IHostingEnvironment env, ILogger<CategoriesController> log)
        {
            this.vegeRepository = vegeRepository;
            this.env = env;
            this.log = log;
        }
        // POST api/values
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Category category)
        {
            Result<Category> result = new Result<Category>();
            try
            {
                var newCate = await this.vegeRepository.AddCategory(category);
                result.body = newCate;
                if (newCate != null)
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

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery]string state)
        {
            Result<IEnumerable<Category>> result = new Result<IEnumerable<Category>>();
            try
            {
                result.body = await this.vegeRepository.GetAllCategories(state);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.body = null;
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}/pictures/{picPath}")]
        public async Task<IActionResult> RemovePic(int id, string picPath)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var succ = await this.vegeRepository.RemoveCategoryPic(id);
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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var fullpath = await this.vegeRepository.RemoveCategory(id);
                if (!string.IsNullOrEmpty(fullpath))
                {
                    var path = fullpath.Substring(fullpath.LastIndexOf('/'));
                    var p = Path.Combine(this.env.WebRootPath, "upload", path);
                    if (System.IO.File.Exists(p))
                    {
                        System.IO.File.Delete(p);
                    }
                }
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
        [HttpPut]
        public async Task<IActionResult> UpdateCate(int id, [FromBody]Category cate)
        {
            Result<bool> result = new Result<bool>();

            try
            {
                if (await this.vegeRepository.UpdateCate(cate))
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
        [HttpPatch("{id}")]
        public async Task<IActionResult> patchCate(int id, [FromBody]JsonPatchDocument<Category> patchDoc)
        {
            var result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.patchCate(id, patchDoc))
                {
                    result.body = true;
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
        [HttpPut("reorder/{id1}/{id2}")]
        public async Task<IActionResult> ReorderCate(int id1, int id2)
        {
            var result = new Result<bool>();
            try
            {
                if (await this.vegeRepository.reorderCate(id1, id2))
                {
                    result.body = true;
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
