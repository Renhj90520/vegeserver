﻿using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vege.Models;

namespace Vege.Utils
{
    public class WXHelper
    {
        public async static Task<WXConfig> getWxConfig(string appId, string appsecret, string server, string mch_id, string key, IMemoryCache cache)
        {
            WXConfig config = new WXConfig();
            var ticket = await getTicket(cache, appId, appsecret);
            var nonceStr = Guid.NewGuid().ToString();

            config.appId = appId;
            config.timestamp = GenerateTimeStamp();
            config.signature = GenerateSignature(ticket, nonceStr, config.timestamp, server);
            config.nonceStr = nonceStr;
            config.key = key;
            config.mch_id = mch_id;
            return config;
        }

        private static async Task<string> getTicket(IMemoryCache cache, string appId, string appsecret)
        {
            string ticket;
            if (cache.TryGetValue("jsapi_ticket", out ticket))
            {
                return ticket;
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    var tokenRes = await client.GetAsync("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appId + "&secret=" + appsecret);
                    if (tokenRes.IsSuccessStatusCode)
                    {
                        var tokenresultStr = await tokenRes.Content.ReadAsStringAsync();
                        var access_token = JObject.Parse(tokenresultStr)["access_token"].ToString();
                        var ticketRes = await client.GetAsync("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi");
                        if (ticketRes.IsSuccessStatusCode)
                        {
                            var ticketresultStr = await ticketRes.Content.ReadAsStringAsync();
                            ticket = JObject.Parse(ticketresultStr)["ticket"].ToString();
                            cache.Set<string>("jsapi_ticket", ticket, TimeSpan.FromSeconds(7100));
                        }
                    }
                }
                return ticket;
            }
        }

        private static string GenerateSignature(string ticket, string nonceStr, string timestamp, string url)
        {
            var info = "jsapi_ticket = " + ticket + " & noncestr = " + nonceStr + " & timestamp = " + timestamp + " & url = " + url;
            return SHA1Encrypter.encrypt(info);
        }

        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}