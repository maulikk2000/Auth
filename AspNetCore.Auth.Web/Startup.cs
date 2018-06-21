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
using AspNetCore.Auth.Web.Helper;
using Microsoft.Extensions.Options;

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
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/auth/signin";
            })
            .AddFacebook(options =>
            {
                options.AppId = Configuration.GetValue<string>("facebook:AppId");
                options.AppSecret = Configuration.GetValue<string>("facebook:AppSecret"); ;
            })
            .AddTwitter(options =>
            {
                //https://stackoverflow.com/questions/46940710/getting-value-from-appsettings-json-in-net-core

                //Oneway of doing thing

                //var twitterSection = Configuration.GetSection("twitter");
                //var twitter = twitterSection.Get<TwitterSection>();

                //// Inject AppIdentitySettings so that others can use too
                //services.Configure<TwitterSection>(twitterSection);

                //=============================================================

                //another way of doing it

                var twitter = Configuration.GetSection("twitter").Get<TwitterSection>();


                options.ConsumerKey = twitter.ConsumerKey;
                options.ConsumerSecret = twitter.ConsumerSecret;


            });
            //.AddCookie();
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
