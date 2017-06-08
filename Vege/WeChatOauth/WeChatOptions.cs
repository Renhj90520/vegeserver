using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.WeChatOauth
{
    public class WeChatOptions : OAuthOptions
    {
        public WeChatOptions()
        {
            AuthenticationScheme = WeChatDefaults.AuthenticationScheme;
            DisplayName = AuthenticationScheme;
            CallbackPath = new PathString("/signin");
            AuthorizationEndpoint = WeChatDefaults.AuthorizationEndpoint;
            TokenEndpoint = WeChatDefaults.TokenEndpoint;
            UserInformationEndpoint = WeChatDefaults.UserInfoEndpoint;

            StateAddition = "#wechat_redirect";
            WeChatScope = InfoScope;
        }

        public string AppId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }

        public string AppSecret
        {
            get { return ClientSecret; }
            set { ClientSecret = value; }
        }

        public string StateAddition { get; set; }
        public string WeChatScope { get; set; }
        public string BaseScope = "snsapi_base";
        public string InfoScope = "snsapi_userinfo";
    }
}
