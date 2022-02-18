using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Exceptions
{
    public static class CustomExceptionHandler  
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                //run middleware'i sonlandırıcıdır. use ise devam ettirir.
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature!=null)
                    {
                        var exception = errorFeature.Error;
                        ErrorDto errorDto = null;
                        if (exception is CustomException)
                        {
                            errorDto = new ErrorDto(exception.Message, true); 
                        }
                        else 
                        {
                            errorDto = new ErrorDto(exception.Message, false);
                        }
                         
                        var response = Response<NoDataDto>.Fail(errorDto, 500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response);
                        //eskiden newtonsoft
                    }
                });
            });
        }
    }
}
