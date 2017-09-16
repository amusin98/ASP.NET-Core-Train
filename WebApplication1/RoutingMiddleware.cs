using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public  class RoutingMiddleware
    {
        private readonly RequestDelegate next;

        public RoutingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string path = context.Request.Path.Value.ToLower();
            if(path == "/index")
            {
                await context.Response.WriteAsync("Home page");
            }
            else if(path == "/about")
            {
                await context.Response.WriteAsync("About page");
            } else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}
