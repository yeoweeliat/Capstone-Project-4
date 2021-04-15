using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grocery.WebApp.Data;
using Grocery.WebApp.Models;
using Grocery.WebApp.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Grocery.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // register services here
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer( Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<MyIdentityUser>(options => // can take in a collection 
            //{
            //        options.SignIn.RequireConfirmedAccount = true; // configure such that: a person can only sign in only if he has an email address confirmed. 
            //        // configure: whether user has been registered by 2factor (2fa) authorization process
            //        // false = the person can login with unconfirmed, dummy accounts
            //        // true = only account that is confirmed through email token authorization can sign in. 
            //        // EMailConfirmed & PhoneNUmberConfirmed control login
            //    })
            //    .AddEntityFrameworkStores<ApplicationDbContext>();


            // use customidentity instead of defaultidentity, we are working with customIdentityModel

            services.AddIdentity<MyIdentityUser, MyIdentityRole>(options => // can take in a collection 
            {
                // Sign In Policy
                options.SignIn.RequireConfirmedAccount = true;

                // Password Policy
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // User Policy
                options.User.RequireUniqueEmail = true; // false = 2 users can have the same email address


            })
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders(); //solves the 2fa-authentication requirement


            //configure the identity application level cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true; //extends by another 20mins(expiretimespan) after user navigates to another page 
            }

            );



            services.AddRazorPages();

            // Register the customized email service - config in appsettings.json
            //in <> we put the service we want to register
            services.AddSingleton<IEmailSender, MyEmailSender>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<MyIdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error"); //use automatic page handler to throw error page
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection(); // if you enter http, it will redirect to https
            app.UseStaticFiles(); // allow you to use the things in wwwroot

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => // this is where you map 2 different styles of endpoints
            {
                endpoints.MapRazorPages(); //another route, look for razor pages, and map those routes
                // be careful not to have a razor page match a default route eg (dont create home razor page for the home controller)
                // you dont want to have the same naming like that

                endpoints.MapControllerRoute( //mapcontrollerroute = asp.net mvc routing
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

            });

            // seed the applicationdbcontext


            //async then wait for the class task = forces the running async code to run synchronously
            ApplicationDbContextSeed.SeedRolesAsync(roleManager).Wait(); //await = method called asynchronously

            // remove await, and write Wait() at the back



        }
    }
}
