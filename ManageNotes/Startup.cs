using System;
using System.Linq;
using System.Security.Claims;
using ManageNotes.Attributes;
using ManageNotes.Data;
using ManageNotes.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;

namespace ManageNotes
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
            services.AddResponseCaching();
            //services.AddMemoryCache();

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("defualt"));
            });
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
            });

            services.AddElmahIo(o =>
            {
                o.ApiKey = "2033dab8ba56457292b5a9374cf96141";
                o.LogId = new Guid("50efd4d0-424e-400e-827f-1b155f2de7c1");
            });

            services.AddScoped<NoteServices>();
            services.AddScoped<UserServices>();
            services.AddScoped<PaymentServices>();

            services.AddParbad().ConfigureGateways(geteway =>
            {
                geteway.AddParbadVirtual()
                    .WithOptions(optiens =>
                        optiens.GatewayPath = "/MyGeteway");
            })
                .ConfigureHttpContext(builder=> builder.UseDefaultAspNetCore())
                .ConfigureStorage(builder=> builder.UseMemoryCache());

            services.AddDistributedMemoryCache();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "firstcookie";
                    options.ExpireTimeSpan=TimeSpan.FromMinutes(15);
                    options.SlidingExpiration = true;
                    options.LoginPath = "/User/logIn";
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnValidatePrincipal = async context =>
                        {
                            
                                var serialNo = context?.Principal?.Claims.FirstOrDefault(x => x.Type == "SerialNo")?.Value;
                                var role = context?.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                                var id = context?.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                                var applicationContext = context?.HttpContext.RequestServices.GetRequiredService<ApplicationContext>();
                                
                                if(id !=null)
                                {
                                    var user = await applicationContext.Users.FindAsync(int.Parse(id));
                                    if ((role!=null && (int)user.Role != int.Parse(role)) ||
                                        (serialNo!=null && user.SerialNo != serialNo))
                                    {
                                        context.RejectPrincipal();
                                        //return;
                                    }
                                }
                            }
                    };
                });
            
            services.AddControllersWithViews(options =>
                {
                    options.Filters.Add(typeof(LogAttribute));
                })
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json","My API V1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseElmahIo();
            
            app.UseResponseCaching();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseParbadVirtualGateway();
        }
    }
}