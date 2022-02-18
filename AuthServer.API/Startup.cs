using AuthServer.Core.Configurations;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using SharedLibrary.Extensions;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.API
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
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>(); 
            services.AddScoped<ITokenService, TokenService>(); 
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>),typeof(ServiceGeneric<,>)); 
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options => 
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"),
                    sqlOptions=> {
                        sqlOptions.MigrationsAssembly("AuthServer.Data"); 
                    });   
            });

            services.AddIdentity<UserApp, IdentityRole>(option => 
             {
                 option.User.RequireUniqueEmail = true;
                 option.Password.RequireNonAlphanumeric = false; 
             }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //DI container'a bir nesne geçtik  
            //Options pattern -> DI üzerinden appsetting'deki datalara ulaşmaya denir.
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption")); 
           
            services.Configure<List<Client>>(Configuration.GetSection("Client"));

            //productcontrollera istek yapıldığında token doğrulama da yapıyor
            //hem token üreten hem de doğrulama yapaım bir api
            services.AddAuthentication(option =>
            {
                //birden fazla üyelik sistemi olan api'lerde
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;            
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opts=> {
                //cookie bazlı mı yoksa jwt bazlı mı yapılacak? jwt olduğunu burada yazıyoruz
                //TokenValidationParameters ile doğrulamada geçerli olacak parametreleri belirtiyoruz.
                //customtokenoption'a mapleme
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer, 
                    ValidAudience = tokenOptions.Audience[0], 
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, 
                    ValidateAudience = true,
                    ValidateIssuer=true, 
                    ValidateLifetime = true,
                    ClockSkew=TimeSpan.Zero 
                    //farklı serverlara kurduğumuz api'nin kurulan serverlar ile
                    //saat farkını tölere etmek için 5 dk ekler. biz onu sıfıra 
                };
            }); 

            services.AddControllers().AddFluentValidation(options =>
            {
                //bu assemblyde bulunan AbstractValidator'ı miras alan sınıflarda uygulanacak
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.UseCustomValidationResponse(); 
            services.AddControllers();
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
