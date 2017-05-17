using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IHostingEnvironment env;
        public UploadController(IHostingEnvironment env)
        {
            this.env = env;
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            long size = file.Length;
            var fileName = file.FileName;
            var extension = fileName.Substring(fileName.LastIndexOf('.'));

            var filePath = Path.Combine(this.env.WebRootPath, "upload", DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { path = filePath });
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
