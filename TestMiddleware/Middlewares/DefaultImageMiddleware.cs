using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace TestMiddleware.Middlewares
{
    /// <summary>
    /// 默认图片中间件
    /// </summary>
    public class DefaultImageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _host;
        private readonly IOptions<DefaultImageOptions> _options;
        public DefaultImageMiddleware(RequestDelegate next, IWebHostEnvironment host, IOptions<DefaultImageOptions> options)
        {
            _next = next;
            _host = host;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                //根据请求类型判断
                var accept = context.Request.Headers["Accept"].ToString();
                //根据文件扩展名判断
                var xpath = context.Request.Path.Value;
                if (!string.IsNullOrEmpty(accept) && accept.ToLower().StartsWith("image") ||
                    !string.IsNullOrEmpty(xpath) && (xpath.ToLower().EndsWith(".png") ||
                                                     (xpath.ToLower().EndsWith(".jpg")) ||
                                                     xpath.ToLower().EndsWith(".jpeg") ||
                                                     xpath.ToLower().EndsWith(".gif") ||
                                                     xpath.ToLower().EndsWith(".webp") ||
                                                     xpath.ToLower().EndsWith(".bmp") ||
                                                     xpath.ToLower().EndsWith(".tiff")))
                {
                    await SetDefaultImage(context);
                }
            }
        }

        /// <summary>
        /// 设置默认图片
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task SetDefaultImage(HttpContext context)
        {
            try
            {
                //更改状态码为响应成功
                context.Response.StatusCode = StatusCodes.Status200OK;
                //读取默认图片
                var path = $"{_host.WebRootPath}\\{_options.Value.ImageUrl}";
                await using var file = new FileStream(path,FileMode.Open,FileAccess.ReadWrite);
                var bites = new byte[file.Length];
                await file.ReadAsync(bites.AsMemory(0, bites.Length));
                //响应输出默认图片
                await context.Response.Body.WriteAsync(bites.AsMemory(0, bites.Length));
            }
            catch (Exception e)
            {
                await context.Response.WriteAsync(e.Message);
            }
        }
    }
}