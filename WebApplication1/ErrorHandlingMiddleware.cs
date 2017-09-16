using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await next.Invoke(context);
            if(context.Response.StatusCode == 403)
            {
                await context.Response.WriteAsync("Access denied!");
            }
            else if(context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync("Not found!");
            }
        } 
    }
}
