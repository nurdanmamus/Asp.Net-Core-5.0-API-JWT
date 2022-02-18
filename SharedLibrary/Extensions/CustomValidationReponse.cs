﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomValidationReponse 
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            //fluent validation'ın default olarak döndüğü çıktıyı override edecez.
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                    ErrorDto errorDto = new ErrorDto(errors.ToList(), true); 
                    return new BadRequestObjectResult(Response<NoContentResult>.Fail(errorDto, 400));
                };
            });
        }
    }
}
