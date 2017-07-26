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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment env;
        private IVegeRepository vegeRepository;
        private IConfigurationRoot config;
        private ILogger<UploadController> log;

        public UploadController(IHostingEnvironment env, IVegeRepository vegeRepository, IConfigurationRoot config, ILogger<UploadController> log)
        {
            this.env = env;
            this.vegeRepository = vegeRepository;
            this.config = config;
            this.log = log;
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
                result.body = new { Path = this.config.GetValue<string>("Server") + path };
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

        [HttpDelete("clean")]
        public IActionResult CleanPictures()
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var folder = Path.Combine(this.env.WebRootPath, "upload");
                if (Directory.Exists(folder))
                {
                    var files = Directory.GetFiles(folder);
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var file in files)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                }
                result.state = 1;
                result.body = true;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.StackTrace);
            }

            return Ok(result);
        }
    }
}
