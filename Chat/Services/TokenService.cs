using Chat.DTOs;
using Chat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chat.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ChatContext _context; 

        public TokenService(IConfiguration configuration, ChatContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public DTOLoginResponse GenerarToken(Usuario credencialesUsuario)
        {
            // Los claims construyen la información que va en el payload del token
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, credencialesUsuario.Email),
                //new Claim(ClaimTypes.Role, credencialesUsuario.Rol ?? "admin"),
            };

            // actualizar LastConnection 
            credencialesUsuario.LastConnection = DateTime.Now;

            // Guardar los cambios en la base de datos
            _context.Entry(credencialesUsuario).State = EntityState.Modified;
            _context.SaveChanges(); // Asegúrate de que estás usando SaveChanges() síncrono, no SaveChangesAsync() para este caso específico


            // Necesitamos la clave de generación de tokens
            var clave = _configuration["ClaveJWT"];
            // Fabricamos el token
            var claveKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
            var signinCredentials = new SigningCredentials(claveKey, SecurityAlgorithms.HmacSha256);
            // Le damos características
            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signinCredentials
            );

            // Lo pasamos a string para devolverlo
            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new DTOLoginResponse()
            {
                Token = tokenString,
                Email = credencialesUsuario.Email,
                Rol = credencialesUsuario.Rol,
                Nombre =credencialesUsuario.Nombre,
                Id = credencialesUsuario.Id,
                Avatar = credencialesUsuario.Avatar,
                Room = credencialesUsuario.Room,
                LastConnection= credencialesUsuario.LastConnection,

            };
        }
    }
}
