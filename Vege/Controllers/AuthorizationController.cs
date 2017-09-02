using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vege.WeChatOauth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Vege.Models;
using Vege.Repositories;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vege.DTO;
using Vege.Utils;
using Microsoft.Extensions.Logging;
using WxPayAPI;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("[controller]")]
    public class AuthorizationController : Controller
    {
        private IConfigurationRoot config;
        private IVegeRepository vegeRepository;
        private ILogger<AuthorizationController> log;

        public AuthorizationController(IConfigurationRoot config, IVegeRepository vegeRepository, ILogger<AuthorizationController> log)
        {
            this.config = config;
            this.vegeRepository = vegeRepository;
            this.log = log;
        }
        public IActionResult Redirect()
        {
            Dictionary<string, string> ps = new Dictionary<string, string>();
            ps.Add("appid", WxPayConfig.APPID);
            ps.Add("redirect_uri", this.config.GetValue<string>("Server") + "/authorization/getuserinfo");
            ps.Add("response_type", "code");
            ps.Add("scope", WeChatDefaults.BaseScope);
            ps.Add("state", "123");

            var url = WeChatDefaults.AuthorizationEndpoint + QueryString.Create(ps).ToString() + WeChatDefaults.StateAddition;
            return Redirect(url);
        }

        [HttpGet("getuserinfo")]
        public async Task<IActionResult> GetUserInfo([FromQuery]string code, [FromQuery]string state)
        {
            Result<string> result = new Result<string>();
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var tokenRequestParameters = new Dictionary<string, string>()
                    {
                        {"appid",WxPayConfig.APPID },
                        {"secret",WxPayConfig.APPSECRET },
                        {"code",code },
                        {"grant_type","authorization_code" }
                    };

                    var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, WeChatDefaults.TokenEndpoint);

                    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    requestMessage.Content = requestContent;
                    using (var client = new HttpClient())
                    {
                        var response = await client.SendAsync(requestMessage);

                        if (response.IsSuccessStatusCode)
                        {
                            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                            string ErrCode = payload.Value<string>("errcode");
                            string ErrMsg = payload.Value<string>("errmsg");

                            if (!string.IsNullOrEmpty(ErrCode) || !string.IsNullOrEmpty(ErrMsg))
                            {
                                return Unauthorized();
                            }
                            var access_token = payload.Value<string>("access_token");
                            var openid = payload.Value<string>("openid");

                            //check if exists
                            if (await this.vegeRepository.CheckUserExists(openid))
                            {
                                return Redirect(this.config["Server"] + "/login/" + "?openid=" + openid);
                            }
                            else
                            {
                                var userinfo = await getUserInfo(access_token, openid, WeChatDefaults.UserInfoEndpoint);
                                if (!string.IsNullOrEmpty(userinfo))
                                {
                                    var user = JObject.Parse(userinfo);
                                    User newUser = new User()
                                    {
                                        Name = user.Value<string>("nickname"),
                                        OpenId = openid,
                                        City = user.Value<string>("city"),
                                        Province = user.Value<string>("province"),
                                        Sex = user.Value<int>("sex")
                                    };
                                    if ((await this.vegeRepository.AddUser(newUser)) != null)
                                    {
                                        return Redirect(this.config["Server"] + "/login/" + "?openid=" + openid);
                                    }
                                    else
                                    {
                                        result.state = 0;
                                        result.message = "添加用户失败";
                                    }
                                }
                                else
                                {
                                    result.state = 0;
                                    result.message = "获取用户信息失败";
                                }
                            }

                        }
                        else
                        {
                            result.state = 0;
                            result.message = "";
                        }
                    }
                }
                else
                {
                    result.state = 0;
                    result.message = "获取access_token失败";
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.state = 0;
                result.message = ex.Message;
                log.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                return Ok(result);
            }
        }
        [HttpPost("gettoken")]
        public async Task<IActionResult> GetToken([FromBody]OpenIdWrapper openidWrapper)
        {
            Result<string> result = new Result<string>();

            try
            {
                if (await this.vegeRepository.CheckUserExists(openidWrapper.Openid))
                {
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub,openidWrapper.Openid),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                    };
                    JwtSecurityToken token = createToken(claims);
                    result.state = 1;
                    result.body = new JwtSecurityTokenHandler().WriteToken(token);
                }
                else
                {
                    result.state = 0;
                    result.message = "用户不存在";
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
        [HttpPatch("updateuserinfo/{openid}")]
        public async Task<IActionResult> updateUserInfo(string openid)
        {
            var result = new Result<User>();
            try
            {
                Dictionary<string, string> ps = new Dictionary<string, string>();
                ps.Add("grant_type", "client_credential");
                ps.Add("appid", WxPayConfig.APPID);
                ps.Add("secret", WxPayConfig.APPSECRET);

                var url = WeChatDefaults.AccessTokenEndpoint + QueryString.Create(ps).ToString();

                using (HttpClient client = new HttpClient())
                {
                    var tokenResp = await client.GetAsync(url);

                    if (tokenResp.IsSuccessStatusCode)
                    {
                        var tokenStr = await tokenResp.Content.ReadAsStringAsync();
                        log.LogDebug("获取access_token返回结果：" + tokenStr);
                        var jobject = JObject.Parse(tokenStr);

                        var access_token = jobject.Value<string>("access_token");
                        var infoStr = await this.getUserInfo(access_token, openid, WeChatDefaults.UserInfoGetEndPoint);
                        log.LogDebug("获取用户信息返回结果：" + infoStr);

                        var user = JObject.Parse(infoStr);
                        User newUser = new User()
                        {
                            NickName = user.Value<string>("nickname"),
                            OpenId = openid,
                            City = user.Value<string>("city"),
                            Province = user.Value<string>("province"),
                            Sex = user.Value<int>("sex")
                        };

                        await this.vegeRepository.updateUserInfo(newUser, result);
                    }
                    else
                    {
                        result.state = 0;
                        result.message = "获取access_token失败";
                    }
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


        private JwtSecurityToken createToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: this.config["Server"],
                audience: this.config["Server"],
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds,
                claims: claims
                );
            return token;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginWrapper login)
        {
            Result<string> result = new Result<string>();
            try
            {
                if (await this.vegeRepository.CheckUserExists(login.UserName, MD5Encrypter.GetMD5Hash(login.Password)))
                {
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub,login.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                    };
                    JwtSecurityToken token = createToken(claims);
                    result.state = 1;
                    result.body = new JwtSecurityTokenHandler().WriteToken(token);
                }
                else
                {
                    result.state = 0;
                    result.message = "登录失败，用户名或密码错误!";
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

        private async Task<string> getUserInfo(string access_token, string openid, string endpoint)
        {
            var queryBuilder = new QueryBuilder()
            {
                {"access_token",access_token },
                {"openid",openid },
                {"lang","zh_CN" }
            };
            var infoRequest = endpoint + queryBuilder.ToString();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(infoRequest);

                if (!res.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failure to retrieve WeChat user information ({res.StatusCode}) Please check if the authentication information is correct and corresponding WeChat Graph API is enabled.");
                }

                return await res.Content.ReadAsStringAsync();
            }
        }

        [HttpGet("wxpaynotify")]
        public IActionResult notifyWxPay()
        {
            return Ok();
        }

        [HttpGet("getclientip")]
        public IActionResult getClientIp()
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return Ok(new { ip = ip });
        }

        [HttpGet("checktime")]
        public IActionResult checkTime()
        {
            var result = new Result<bool>();
            try
            {
                DateTime now = DateTime.Now;
                DateTime begin = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                DateTime end = new DateTime(now.Year, now.Month, now.Day, 20, 30, 0);
                result.body = now.CompareTo(begin) >= 0 && now.CompareTo(end) <= 0;
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
    }
}
