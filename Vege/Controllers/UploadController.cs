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
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment env;
        private IVegeRepository vegeRepository;
        private IConfigurationRoot config;

        public UploadController(IHostingEnvironment env, IVegeRepository vegeRepository, IConfigurationRoot config)
        {
            this.env = env;
            this.vegeRepository = vegeRepository;
            this.config = config;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile upload)
        {
            Result<Object> result = new Result<object>();
            try
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
                result.Body = new { Path = this.config.GetValue<string>("PictureServer") + path };
                result.State = 1;
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
