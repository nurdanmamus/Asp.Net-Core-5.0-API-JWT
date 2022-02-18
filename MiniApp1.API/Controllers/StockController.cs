using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniApp1.API.Controllers
{
    [Authorize]  
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase 
    {
        [HttpGet] 
        public IActionResult GetStock()
        {  
            //token payload üzerinden name alanını alıyoruz.
            var userName = HttpContext.User.Identity.Name;    
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock UserName:{userName} UserId: {userIdClaim.Value}");
            //veri tabanından userName ya da userId alanları üzerinden gerekli dataları çek
        }
    }
}
