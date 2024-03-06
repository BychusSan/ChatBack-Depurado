using Chat.DTOs;
using Chat.Models;
using Chat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using Chat.Classes;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly ChatContext _context;
        private readonly HashService _hashService;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;



        public UsuarioController(ChatContext context, HashService hashService, TokenService tokenService, EmailService emailService)
        {
            _context = context;
            _hashService = hashService;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        // lista compartida para rastrear usuarios conectados
        //public static List<Usuario> UsuariosConectados { get; } = new List<Usuario>();

        private static List<Usuario> UsuariosConectados = new List<Usuario>();
        private static readonly object lockObject = new object();

        #region GET

        [HttpGet]
        public async Task<ActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuarioById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
        #endregion

        #region HASH

        //[HttpPost("hash/nuevousuario")]
        //public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] DTOUsuario usuario)
        //{

        //    var existeUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
        //    if (existeUsuario != null)
        //    {
        //        return BadRequest("El email ya está en uso.");
        //    }
        //    var existeUsuario2 = await _context.Usuarios.FirstOrDefaultAsync(x => x.Nombre == usuario.Nombre);
        //    if (existeUsuario2 != null)
        //    {
        //        return BadRequest("El nombre ya está en uso.");
        //    }
        //    var resultadoHash = _hashService.Hash(usuario.Password);
        //    var newUsuario = new Usuario
        //    {
        //        Nombre = usuario.Nombre,
        //        Email = usuario.Email,
        //        Password = resultadoHash.Hash,
        //        Salt = resultadoHash.Salt,
        //        Rol = usuario.Rol,
        //        Room = usuario.Room,
        //        Avatar = usuario.Avatar, // Aquí asignamos la URL del avatar
        //        FechaRegistro = usuario.FechaRegistro,
        //    };

        //    await _context.Usuarios.AddAsync(newUsuario);
        //    await _context.SaveChangesAsync();

        //    return Ok(newUsuario);
        //}


        //[HttpPost("hash/nuevousuario")]
        //public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] DTOUsuario usuario)
        //{
        //    if (!EsFormatoEmailValido(usuario.Email))
        //    {
        //        return BadRequest("El formato del correo electrónico no es válido.");
        //    }

        //    var existeUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
        //    if (existeUsuario != null)
        //    {
        //        return BadRequest("El correo electrónico ya está en uso.");
        //    }

        //    var existeUsuario2 = await _context.Usuarios.FirstOrDefaultAsync(x => x.Nombre == usuario.Nombre);
        //    if (existeUsuario2 != null)
        //    {
        //        return BadRequest("El nombre de usuario ya está en uso.");
        //    }

        //    var resultadoHash = _hashService.Hash(usuario.Password);
        //    var newUsuario = new Usuario
        //    {
        //        Nombre = usuario.Nombre,
        //        Email = usuario.Email,
        //        Password = resultadoHash.Hash,
        //        Salt = resultadoHash.Salt,
        //        Rol = usuario.Rol,
        //        Room = usuario.Room,
        //        Avatar = usuario.Avatar,
        //        FechaRegistro = usuario.FechaRegistro,
        //    };

        //    await _context.Usuarios.AddAsync(newUsuario);
        //    await _context.SaveChangesAsync();

        //    //await _emailService.EnviarEmailBienvenida(newUsuario.Email, newUsuario);
        //    await _emailService.EnviarCorreo(newUsuario.Email, newUsuario);

        //    return Ok(newUsuario);
        //}

        private bool EsFormatoEmailValido(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }


        [HttpPost("hash/nuevousuario")]
        public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] DTOUsuario usuario)
        {
            if (!EsFormatoEmailValido(usuario.Email))
            {
                return BadRequest("El formato del correo electrónico no es válido.");
            }

            var existeUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (existeUsuario != null)
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            var existeUsuario2 = await _context.Usuarios.FirstOrDefaultAsync(x => x.Nombre == usuario.Nombre);
            if (existeUsuario2 != null)
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            var resultadoHash = _hashService.Hash(usuario.Password);
            var newUsuario = new Usuario
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Password = resultadoHash.Hash,
                Salt = resultadoHash.Salt,
                Rol = usuario.Rol,
                Room = usuario.Room,
                Avatar = usuario.Avatar,
                FechaRegistro = DateTime.Now
            };

            await _context.Usuarios.AddAsync(newUsuario);
            await _context.SaveChangesAsync();


            try
            {
                //await _emailService.EnviarCorreoElectronico(usuario.Email, "¡Bienvenido a ConnecTalk!", $"Bienvenido a ConnecTalk!\n\nHola {usuario.Nombre}, tus datos de inicio de sesión son:\n\nNombre de usuario: {usuario.Nombre}\nEmail: {usuario.Email}\nRol: {usuario.Rol}\nPassword: {usuario.Password}\n\nGracias por unirte a nosotros.");
                await _emailService.EnviarCorreoElectronico(usuario.Email, newUsuario, usuario.Password);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al enviar correo electrónico de bienvenida.");
            }

            return Ok(newUsuario);
        }



        [HttpPost("hash/checkusuario")]
        public async Task<ActionResult> CheckUsuarioHash([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized("usuario no existe");
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

        #endregion

        #region LOGIN


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return BadRequest();
            }

            var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                UsuariosConectados.Add(usuarioDB);

                var response = _tokenService.GenerarToken(usuarioDB);
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }



        [HttpGet("connectedUsers")]
        public ActionResult<IEnumerable<Usuario>> GetConnectedUsers()
        {
            lock (lockObject)
            {
                string userEmail = ObtenerEmailUsuario();

                if (!string.IsNullOrEmpty(userEmail))
                {
                    var usuarioExistente = UsuariosConectados.FirstOrDefault(u => u.Email == userEmail);

                    if (usuarioExistente == null)
                    {
                        var nuevoUsuario = new Usuario
                        {
                            Email = userEmail
                        };

                        UsuariosConectados.Add(nuevoUsuario);
                    }
                    if (usuarioExistente != null)
                    {
                        DisconnectUserByEmail(userEmail);
                    }
                }
                return Ok(UsuariosConectados);
            }
        }

        // deberia de estar funcionando para seleccionar email
        private string ObtenerEmailUsuario()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string userEmail = HttpContext.User.Identity.Name;
                return userEmail;
            }

            return null;
        }



        [HttpDelete("disconnectUser")]
        public ActionResult DisconnectUserByEmail([FromQuery] string userEmail)
        {
            lock (lockObject)
            {
                var usuariosDesconectar = UsuariosConectados.Where(u => u.Email == userEmail).ToList();

                if (usuariosDesconectar.Any())
                {
                    UsuariosConectados.RemoveAll(u => u.Email == userEmail);

                    return Ok($"Usuarios con correo electrónico {userEmail} desconectados correctamente.");
                }
                else
                {
                    return NotFound($"No se encontraron usuarios con correo electrónico {userEmail} en la lista de usuarios conectados.");
                }
            }
        }

        [HttpDelete("disconnectAllUsers")]
        public ActionResult DisconnectAllUser()
        {
            lock (lockObject)
            {
                var usuariosDesconectar = UsuariosConectados.ToList();

                if (usuariosDesconectar.Any())
                {
                    UsuariosConectados.Clear();

                    return Ok("Todos los usuarios han sido desconectados correctamente.");
                }
                else
                {
                    return NotFound("La lista de usuarios conectados está vacía.");
                }
            }
        }

        [HttpPut("updateUserRoom")]
        public ActionResult UpdateUserRoleByEmail([FromQuery] string userEmail, [FromQuery] string newRoom)
        {
            lock (lockObject)
            {
                var usuarioAActualizar = UsuariosConectados.FirstOrDefault(u => u.Email == userEmail);

                if (usuarioAActualizar != null)
                {
                    usuarioAActualizar.Room = newRoom;

                    return Ok($"Rol del usuario con correo electrónico {userEmail} actualizado correctamente a {newRoom}.");
                }
                else
                {
                    return NotFound($"No se encontró un usuario con correo electrónico {userEmail} en la lista de usuarios conectados.");
                }
            }
        }


        #endregion

        #region UPDATE/DELETE

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUsuarios(int id, [FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FindAsync(id);

            if (usuarioDB == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Actualizar propiedades del usuario según los datos recibidos en el DTO
            usuarioDB.Nombre = usuario.Nombre;
            usuarioDB.Email = usuario.Email;
            usuarioDB.Rol = usuario.Rol;
            usuarioDB.Room = usuario.Room;
            usuarioDB.Avatar = usuario.Avatar; // Actualizar la URL del avatar si es necesario
            usuarioDB.FechaUpdate = DateTime.Now;

            _context.Entry(usuarioDB).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(usuarioDB);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error al intentar actualizar el usuario");
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuarios(int id)
        {
            var usuarioDB = await _context.Usuarios.FindAsync(id);

            if (usuarioDB == null)
            {
                return NotFound("Usuario no encontrado");
            }

            _context.Usuarios.Remove(usuarioDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        #endregion


        #region CAMBIO PASSWORD

        //VERSION CAMBIO PASS SOLICITANDO CONTRASEÑA ACTUAL, NO BORRAR

        //[HttpPut("cambioPasswordHASH")]
        //public async Task<ActionResult> ChangePassword([FromBody] DTOUsuarioNuevaContraseña usuario)
        //{
        //    var usuarioDB = await _context.Usuarios.AsTracking().FirstOrDefaultAsync(x => x.Email == usuario.Email);
        //    if (usuarioDB == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // hash y salt actuales
        //    var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
        //    if (usuarioDB.Password != resultadoHash.Hash)
        //    {
        //        return Unauthorized();
        //    }

        //    // hash y salt nuevos
        //    var nuevoHash = _hashService.Hash(usuario.nuevaPassword);
        //    usuarioDB.Password = nuevoHash.Hash;
        //    usuarioDB.Salt = nuevoHash.Salt;

        //    //_context.Update(usuarioDB);       // No es obligatorio ponerlo, lo actualia automáticamente
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

        [HttpPut("cambioPasswordHASH")]
        public async Task<ActionResult> ChangePassword([FromBody] DTOUsuarioNuevaContraseña usuario)
        {
            var usuarioDB = await _context.Usuarios.AsTracking().FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized();
            }

            // No se verifica el password actual

            // hash y salt nuevos
            var nuevoHash = _hashService.Hash(usuario.nuevaPassword);
            usuarioDB.Password = nuevoHash.Hash;
            usuarioDB.Salt = nuevoHash.Salt;

            //_context.Update(usuarioDB);       // No es obligatorio ponerlo, lo actualiza automáticamente
            await _context.SaveChangesAsync();

            return Ok();
        }




        #endregion


    }
}
