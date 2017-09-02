using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.WeChatOauth
{
    public static class WeChatDefaults
    {
        public const string AuthenticationScheme = "WeChat";
        public static readonly string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/oauth2/authorize";
        public static readonly string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";
        public static readonly string UserInfoEndpoint = "https://api.weixin.qq.com/sns/userinfo";
        public static readonly string AccessTokenEndpoint = "https://api.weixin.qq.com/cgi-bin/token";
        public static readonly string UserInfoGetEndPoint = "https://api.weixin.qq.com/cgi-bin/user/info";
        public const string BaseScope = "snsapi_base";
        public const string InfoScope = "snsapi_userinfo";
        public const string StateAddition = "#wechat_redirect";
    }
}
