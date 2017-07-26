using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Vege.Models;
using Microsoft.Extensions.Configuration;
using Vege.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http.Extensions;
using Vege.WeChatOauth;
using System.Net.Http;
using Vege.Repositories;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class WeChatController : Controller
    {
        private IConfigurationRoot config;
        private IMemoryCache cache;
        private IVegeRepository vegeRepository;
        private ILogger<WeChatController> log;

        public WeChatController(IConfigurationRoot config, IMemoryCache cache, IVegeRepository vegeRepository, ILogger<WeChatController> log)
        {
            this.config = config;
            this.cache = cache;
            this.vegeRepository = vegeRepository;
            this.log = log;
        }

        [HttpGet("getwxconfig")]
        public async Task<IActionResult> getWxConfig()
        {
            var result = new Result<WXConfig>();

            try
            {
                var appid = this.config["AppId"];
                var appsecret = this.config["AppSecret"];
                var server = this.config["Server"];
                var mch_id = this.config["mch_id"];
                var key = this.config["key"];
                result.body = await WXHelper.getWxConfig(appid, appsecret, server, mch_id, key, cache);
                result.state = 1;
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [HttpPost("refund/{id}")]
        public async Task<IActionResult> Refund(int id, [FromBody] RefundWrapper wrapper)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                await vegeRepository.refundOrder(id, wrapper, result);
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
