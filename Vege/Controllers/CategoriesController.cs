using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.Models;
using Vege.Repositories;
using System.IO;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private IVegeRepository vegeRepository;
        private IHostingEnvironment env;

        public CategoriesController(IVegeRepository vegeRepository, IHostingEnvironment env)
        {
            this.vegeRepository = vegeRepository;
            this.env = env;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Category category)
        {
            Result<Category> result = new Result<Category>();
            try
            {
                var newCate = await this.vegeRepository.AddCategory(category);
                result.Body = newCate;
                if (newCate != null)
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

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            Result<IEnumerable<Category>> result = new Result<IEnumerable<Category>>();
            try
            {
                result.Body = await this.vegeRepository.GetAllCategories();
                result.State = 1;
            }
            catch (Exception ex)
            {
                result.Body = null;
                result.State = 0;
                result.Message = ex.Message;
            }
            return Ok(result);
        }

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
                result.State = 1;
            }
            catch (Exception ex)
            {
                result.State = 0;
                result.Message = ex.Message;
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCate(int id, [FromBody]Category cate)
        {
            Result<bool> result = new Result<bool>();

            try
            {
                if (await this.vegeRepository.UpdateCate(cate))
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
