using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.IO;
using Microsoft.Extensions.Logging;
using Project1;

namespace WebApplication1
{
    public class Startup
    {
        IServiceCollection serv;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            serv = services;
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.CookieName = ".MyApp.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
            });
            services.AddDirectoryBrowser();
            var servs = services.ToList();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
        {
            factory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
            var logger = factory.CreateLogger("FileLogger");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.Use(async (context, next) =>
            {
                context.Items.Add("text","Text from http context items");
                await next.Invoke();
            });
            
            app.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>Services</h1>");
                sb.Append("<table>");
                sb.Append("<tr><th>Тип</th><th>Lifetime</th><th>Реализация</th></tr>");
                foreach (var svc in serv)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                context.Response.ContentType = "text/html;charset=utf-8";
                await context.Response.WriteAsync(sb.ToString());
            });
        }

        public Task SendResponseAsync(IDictionary<string, object> env)
        {
            var reqH = (IDictionary<string, string[]>)env["owin.RequestHeaders"];
            string browser = reqH["User-Agent"][0];
            foreach (var item in reqH)
            {
                browser += "<br>" + item.Key + " - > " + item.Value[0];
            }
            
            byte[] bytes = Encoding.UTF8.GetBytes(browser);
            var stream = (Stream)env["owin.ResponseBody"];
            return stream.WriteAsync(bytes, 0 , bytes.Length);
        }

        private static void HandleId(IApplicationBuilder app)
        {
            app.Run(async context => 
            {
                await context.Response.WriteAsync("id = 5");
            });
        }

        public static void Index(IApplicationBuilder app)
        {
            app.Run(async (context) => {
                await context.Response.WriteAsync("Index");
            });
        }


        public static void About(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("About");
            });
        }
    }
}
