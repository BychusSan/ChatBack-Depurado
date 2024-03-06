namespace Chat.DTOs
{
    public class DTOUsuario
    {
        //public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Rol { get; set; }

        public string? Room { get; set; }

        public string? Avatar { get; set; } = "https://cdn-icons-png.flaticon.com/256/456/456141.png";

        public DateTime? FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaUpdate { get; set; } = DateTime.Now;


    }

    public class DTOUsuarioLinkChangePassword
    {
        public string Email { get; set; }

    }


    public class DTOUsuarioChangePassword
    {
        public string Email { get; set; }
        public string Password { get; set; }
        //public string Enlace { get; set; }
    }

   
        public class DTOUsuarioNuevaContraseña
        {
            public string Email { get; set; }
            //public string Password { get; set; }
            public string nuevaPassword { get; set; }
            //public string Enlace { get; set; }
        }


    public class DTOCorreo
    {
        public string Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Contenido { get; set; }
    }


}
