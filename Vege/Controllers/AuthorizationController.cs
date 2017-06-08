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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Vege.Controllers
{
    [Route("[controller]")]
    public class AuthorizationController : Controller
    {
        private IConfigurationRoot config;
        private HttpClient client;
        private IVegeRepository vegeRepository;

        public AuthorizationController(IConfigurationRoot config, HttpClient client, IVegeRepository vegeRepository)
        {
            this.config = config;
            this.client = client;
            this.vegeRepository = vegeRepository;
        }
        public IActionResult Redirect()
        {
            Dictionary<string, string> ps = new Dictionary<string, string>();
            ps.Add("appid", this.config.GetValue<string>("AppId"));
            ps.Add("redirect_uri", this.config.GetValue<string>("Server") + "/getuserinfo");
            ps.Add("response_type", "code");
            ps.Add("scope", WeChatDefaults.BaseScope);
            ps.Add("state", "123");

            var url = WeChatDefaults.AuthorizationEndpoint + QueryString.Create(ps).ToString() + WeChatDefaults.StateAddition;
            return Redirect(url);
        }

        [HttpGet("getuserinfo")]
        public async Task<IActionResult> GetUserInfo([FromQuery]string code, [FromQuery]string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var tokenRequestParameters = new Dictionary<string, string>()
                {
                    {"appid",this.config.GetValue<string>("AppId") },
                    {"secret",this.config.GetValue<string>("AppSecret") },
                    {"code",code },
                    {"grant_type","authorization_code" }
                };

                var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, WeChatDefaults.TokenEndpoint);

                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Content = requestContent;
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

                    }
                    else
                    {
                        var userinfo = await getUserInfo(access_token, openid);
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
                        }
                    }

                }
                else
                {

                }
            }
            else
            {

            }
            return Ok();
        }

        private async Task<string> getUserInfo(string access_token, string openid)
        {
            var queryBuilder = new QueryBuilder()
            {
                {"access_token",access_token },
                {"openid",openid },
                {"lang","zh_CN" }
            };
            var infoRequest = WeChatDefaults.UserInfoEndpoint + queryBuilder.ToString();

            var res = await client.GetAsync(infoRequest);

            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failure to retrieve WeChat user information ({res.StatusCode}) Please check if the authentication information is correct and corresponding WeChat Graph API is enabled.");
            }

            return await res.Content.ReadAsStringAsync();
        }
    }
}
