using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class TokenDto  
    {
        //JWT-> head,payload,imza kısmı (base64 ile encode edilmiş bir string)
        //decode edilebilir ama değişiklik yapılamsaz.
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public string RefreshTokenExpiration { get; set; } 
    }
}
