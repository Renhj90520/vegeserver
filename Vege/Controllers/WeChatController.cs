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
using WxPayAPI;
using Vege.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.IO;
using Microsoft.AspNetCore.Hosting;

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
        private IHostingEnvironment env;

        public WeChatController(IConfigurationRoot config, IMemoryCache cache, IVegeRepository vegeRepository, ILogger<WeChatController> log, IHostingEnvironment env)
        {
            this.config = config;
            this.cache = cache;
            this.vegeRepository = vegeRepository;
            this.log = log;
            this.env = env;
        }

        //[HttpGet("getwxconfig")]
        //public async Task<IActionResult> getWxConfig()
        //{
        //    var result = new Result<WXConfig>();

        //    try
        //    {
        //        var appid = WxPayConfig.APPID;
        //        var appsecret = WxPayConfig.APPSECRET;
        //        var server = this.config["Server"];
        //        var mch_id = WxPayConfig.MCHID;
        //        var key = WxPayConfig.KEY;
        //        result.body = await WXHelper.getWxConfig(appid, appsecret, server, mch_id, key, cache);
        //        result.state = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.state = 0;
        //        result.message = ex.Message;
        //        log.LogError(ex.Message + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    return Ok(result);
        //}

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

        [HttpPut("prepay/{id}/{totalfee}/{openid}")]
        public async Task<IActionResult> Prepay(int id, int totalfee, string openid)
        {
            var result = new Result<WXConfig>();
            try
            {
                //var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                WxPayData data = new WxPayData(log);
                var wxOrderId = WxPayApi.GenerateOutTradeNo(id);
                data.SetValue("body", "昌吉市肯微年特商行-便利店");
                data.SetValue("out_trade_no", wxOrderId);
                data.SetValue("total_fee", totalfee);
                data.SetValue("trade_type", "JSAPI");
                data.SetValue("openid", openid);
                data.SetValue("notify_url", config["Server"] + "authorization/wxpaynotify");
                var coupon = await vegeRepository.getValidityCoupon();
                if (coupon != null)
                {
                    data.SetValue("goods_tag", coupon.Code);
                    log.LogDebug("goods_tag is : " + coupon.Code);
                }
                WxPayData res = await WxPayApi.UnifiedOrder(data, log, 15);
                if ("SUCCESS" == res.GetValue("return_code").ToString())
                {
                    string prepayid = res.GetValue("prepay_id").ToString();
                    var appid = WxPayConfig.APPID;
                    var appsecret = WxPayConfig.APPSECRET;
                    var key = WxPayConfig.KEY;
                    JsonPatchDocument<Order> orderPatch = new JsonPatchDocument<Order>();
                    var wxorderidoper = new Operation<Order>() { op = "replace", path = "/WXOrderId", value = wxOrderId };
                    orderPatch.Operations.Add(wxorderidoper);
                    if (await this.vegeRepository.UpdateOrder(id, orderPatch))
                    {
                        var origin = Request.Headers["Origin"];
                        log.LogDebug("Origin is : " + origin);

                        var userAgent = Request.Headers["User-Agent"].ToString();
                        log.LogDebug("User agent is : " + userAgent);
                        string server;
                        if (userAgent.Contains("iPhone"))
                        {
                            server = Request.Scheme + "://" + Request.Host + "/login/?openid=" + openid;
                        }
                        else
                        {
                            server = WXHelper.GetAbsoluteUri(Request);
                        }
                        log.LogDebug("Server is : " + server);
                        log.LogDebug("Openid is : " + openid);
                        result.body = await WXHelper.getWxConfig(appid, appsecret, server, prepayid, key, cache, log);
                        result.state = 1;
                    }
                    else
                    {
                        result.state = 0;
                        result.message = "更新微信订单发生错误";
                    }
                }
                else
                {
                    result.state = 0;
                    result.message = res.GetValue("return_msg").ToString();
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

        [HttpPost("coupons")]
        public async Task<IActionResult> AddCoupon([FromBody]Coupon coupon)
        {
            Result<Coupon> result = new Result<Coupon>();
            try
            {
                result.body = await vegeRepository.addCoupon(coupon);
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

        [HttpPatch("coupons/{id}")]
        public async Task<IActionResult> PatchCoupon(int id, [FromBody] JsonPatchDocument<Coupon> patchDoc)
        {
            var result = new Result<bool>();
            try
            {
                await vegeRepository.patchCoupon(id, patchDoc, result);
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(result);
        }

        [HttpPut("coupons/{id}")]
        public async Task<IActionResult> updateCoupon(int id, [FromBody]Coupon coupon)
        {
            var result = new Result<bool>();
            try
            {
                await vegeRepository.updateCoupon(id, coupon, result);
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return Ok(result);
        }

        [HttpGet("coupons")]
        public async Task<IActionResult> getAllCoupons()
        {
            var result = new Result<IEnumerable<Coupon>>();
            try
            {
                result.body = await vegeRepository.getAllCoupons();
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

        [HttpGet("coupons/validity")]
        public async Task<IActionResult> getValidyCoupon()
        {
            var result = new Result<Coupon>();
            try
            {
                result.body = await vegeRepository.getValidityCoupon();
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

        [HttpDelete("coupons/{id}/pictures/{path}")]
        public async Task<IActionResult> RemoveCouponPic(int id, string path)
        {
            var result = new Result<bool>();
            try
            {
                var succ = await vegeRepository.removeCouponPic(id);
                if (succ)
                {
                    var fullPath = Path.Combine(this.env.WebRootPath, "upload", path);
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
