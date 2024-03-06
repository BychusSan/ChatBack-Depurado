namespace Chat.DTOs
{
    public class DTOLoginResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Rol { get; set; }

        public string Room { get; set; }
        public string Nombre { get; set; }

        public string Avatar { get; set; }
        public DateTime? LastConnection { get; set; } = DateTime.Now;


    }
}
