using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Almacen.DTOs;

namespace WebApiAlmacen.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private LoginResponseDTO GenerarToken(UsuarioDTO credencialesUsuario)
        {
            var claims = new List<Claim>()
         {
             new Claim(ClaimTypes.Email, credencialesUsuario.Email),
             new Claim("lo que yo quiera", "cualquier otro valor")
         };

            var clave = _configuration["ClaveJWT"];
            var claveKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
            var signinCredentials = new SigningCredentials(claveKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signinCredentials
            );


            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new LoginResponseDTO()
            {
                Token = tokenString,
                Email = credencialesUsuario.Email
            };
        }
    }
}
