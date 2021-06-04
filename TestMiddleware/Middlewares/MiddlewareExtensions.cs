using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TestMiddleware.Middlewares
{
    /// <summary>
    /// 中间件扩展方法
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// 添加默认图片
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultImage(this IServiceCollection services, Action<DefaultImageOptions> options)
        {
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// 使用默认图片
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultImage(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DefaultImageMiddleware>();
        }
    }
}