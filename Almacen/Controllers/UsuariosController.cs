using Almacen.Models;
using Almacen.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Almacen.DTOs;

namespace WebApiAlmacen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AlmacenContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _dataProtector;
        private readonly HashService _hashService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(AlmacenContext context, IConfiguration configuration,
        IDataProtectionProvider dataProtector, HashService hashService, ILogger<UsuariosController> logger)
        {
            _context = context;
            _configuration = configuration;
            _dataProtector = dataProtector.CreateProtector(configuration["ClaveEncriptacion"]);
            _hashService = hashService;
            _logger = logger;
        }

        [HttpPost("encriptar/nuevoUsuario")]
        public async Task<ActionResult> PostNuevoUsuario([FromBody] UsuarioDTO usuario)
        {
            var passEncriptado = _dataProtector.Protect(usuario.Password);
            var newUsuario = new Usuario
            {
                Email = usuario.Email,
                Password = passEncriptado
            };
            await _context.Usuarios.AddAsync(newUsuario);
            await _context.SaveChangesAsync();

            return Ok(newUsuario);
        }

        [HttpPost("encriptar/checkUsuario")]
        public async Task<ActionResult> PostCheckUserPassEncriptado([FromBody] UsuarioDTO usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized();
            }

            var passDesencriptado = _dataProtector.Unprotect(usuarioDB.Password);
            if (usuario.Password == passDesencriptado)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("hash/nuevoUsuario")]
        public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] UsuarioDTO usuario)
        {
            var resultadoHash = _hashService.Hash(usuario.Password);
            var newUsuario = new Usuario
            {
                Email = usuario.Email,
                Password = resultadoHash.Hash,
                Salt = resultadoHash.Salt
            };

            await _context.Usuarios.AddAsync(newUsuario);
            await _context.SaveChangesAsync();

            return Ok(newUsuario);
        }

        [HttpPost("hash/checkUsuario")]
        public async Task<ActionResult> CheckUsuarioHash([FromBody] UsuarioDTO usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized();
            }

            var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO usuario)
        {
            try
            {
                _logger.LogInformation(usuario.Email + " ha iniciado un login el día " +
                DateTime.Now);
                var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
                if (usuarioDB == null)
                {
                    return BadRequest("Usuario no encontrado.");
                }

                var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
                if (usuarioDB.Password == resultadoHash.Hash)
                {
                    var response = GenerarToken(usuario);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Contraseña incorrecta.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("renovarToken")]
        public async Task<ActionResult> RenovarToken([FromBody] UsuarioDTO usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return BadRequest();
            }

            var response = GenerarToken(usuario);
            return Ok(response);
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

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] UsuarioChangePasswordDTO usuarioChangePasswordDto)
        {
            // Comprobar que el usuario existe en la base de datos
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuarioChangePasswordDto.Email);

            if (usuarioDB == null)
            {
                // Si el usuario no existe, devolver un 401 Unauthorized
                return Unauthorized("Usuario no encontrado");
            }

            // Comprobar que el password actual es correcto
            var resultadoHash = _hashService.Hash(usuarioChangePasswordDto.Password, usuarioDB.Salt);

            if (usuarioDB.Password != resultadoHash.Hash)
            {
                // Si el password actual es incorrecto, devolver un 401 Unauthorized
                return Unauthorized("El password actual es incorrecto");
            }

            // Generar un nuevo hash para el nuevo password
            var nuevoResultadoHash = _hashService.Hash(usuarioChangePasswordDto.NewPassword);

            // Actualizar el hash y el salt en la base de datos
            usuarioDB.Password = nuevoResultadoHash.Hash;
            usuarioDB.Salt = nuevoResultadoHash.Salt;

            // Guardar los cambios en la base de datos
            _context.Usuarios.Update(usuarioDB);
            await _context.SaveChangesAsync();

            // Devolver un 200 OK indicando que el cambio fue exitoso
            return Ok("Password actualizado correctamente");
        }

    }
}