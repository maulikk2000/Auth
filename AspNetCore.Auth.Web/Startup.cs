using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Auth.Web.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace AspNetCore.Auth.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            var users = new Dictionary<string, string> { {"Maulik","khandwala" } };
            services.AddSingleton<IUserService>(new DummyUserService(users));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            })
            //.AddCookie(options =>
            //{
                    
            //    options.LoginPath = "/auth/signin";

            //})
            .AddFacebook(options =>
            {
                options.AppId = Configuration.GetValue<string>("facebook:AppId");
                options.AppSecret = Configuration.GetValue<string>("facebook:AppSecret"); ;
            })
            .AddCookie();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301, 44343));

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
