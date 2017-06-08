using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.WeChatOauth
{
    public static class WeChatAppBuilderExtensions
    {
        public static IApplicationBuilder UseWeChatAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<WeChatMiddleWare>();
        }

        public static IApplicationBuilder UseWeChatAuthentication(this IApplicationBuilder app, WeChatOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<WeChatMiddleWare>(Options.Create(options));
        }
    }
}
