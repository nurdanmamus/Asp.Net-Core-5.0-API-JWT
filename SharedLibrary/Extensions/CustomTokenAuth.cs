using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth  
    {
        //extension methodlar static olur 
        public static void AddCustomTokenAuth(this IServiceCollection services,
            CustomTokenOption tokenOptions) 
        {
            services.AddAuthentication(option =>   
            {
                //birden fazla üyelik sistemi olan api'lerde
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                //cookie bazlı mı yoksa jwt bazlı mı yapılacak? jwt olduğunu burada yazıyoruz
                //TokenValidationParameters ile doğrulamada geçerli olacak parametreleri belirtiyoruz.
                //customtokenoption'a mapleme
               // var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                { 
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,  
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                    //farklı serverlara kurduğumuz api'nin kurulan serverlar ile
                    //saat farkını tölere etmek için 5 dk ekler. biz onu sıfıra 
                };
            });

        }
    }
}
