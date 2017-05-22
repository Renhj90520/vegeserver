using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Vege.Models;
using Vege.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment env;
        private IVegeRepository vegeRepository;
        public UploadController(IHostingEnvironment env, IVegeRepository vegeRepository)
        {
            this.env = env;
            this.vegeRepository = vegeRepository;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile upload)
        {
            long size = upload.Length;
            var fileName = upload.FileName;
            var extension = fileName.Substring(fileName.LastIndexOf('.'));
            string path = @"upload/" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
            var filePath = Path.Combine(this.env.WebRootPath, "upload", DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }

            var picture = await this.vegeRepository.AddPicture(path);

            return Ok(picture);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Result<Object> result = new Result<Object>();
            string path = await this.vegeRepository.GetFilePath(id);
            if (!string.IsNullOrEmpty(path))
            {
                var fullPath = Path.Combine(this.env.WebRootPath, path);
                if (System.IO.File.Exists(Path.Combine(this.env.WebRootPath, path)))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            result.State = 1;
            return Ok(result);
        }
    }
}
