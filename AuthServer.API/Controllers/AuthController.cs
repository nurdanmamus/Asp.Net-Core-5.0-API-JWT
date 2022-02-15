using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")] 
    [ApiController]
    public class AuthController : CustomBaseController 
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }  

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        { 
            return ActionResultInstance(await _authenticationService.CreateTokenAsync(loginDto)); 
        }

        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            return ActionResultInstance(_authenticationService.CreateTokenByClient(clientLoginDto)); 
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {  
            return ActionResultInstance(await _authenticationService.RevokeRefreshToken(refreshTokenDto.Token)); 
        }

        [HttpPost] 
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            return ActionResultInstance(await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.Token));
        }
    } 
}
