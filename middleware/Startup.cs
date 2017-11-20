using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace middleware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            /*app.Use(async (context, next) =>
            {
                var sw = new Stopwatch();
                sw.Start();
                await next();
                sw.Stop();
                await context.Response.WriteAsync(sw.ElapsedMilliseconds.ToString());
            });*/
            app.UseMiddleware<StopwatchMiddleware>();
            app.Use(async (context, next) =>
            {
                Thread.Sleep(10);
                await next();
            });
            app.UseStaticFiles();
        }
    }

    public class StopwatchMiddleware
    {
        private RequestDelegate Next { get; }
        private Stopwatch sw { get; }

        public StopwatchMiddleware(RequestDelegate next)
        {
            Next = next;
            sw = new Stopwatch();
        }

        public async Task Invoke(HttpContext context)
        {
            sw.Reset();
            sw.Start();
            await Next(context);
            sw.Stop();
            await context.Response.WriteAsync(sw.ElapsedMilliseconds.ToString());
        }
    }
}
