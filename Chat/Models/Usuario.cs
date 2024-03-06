using System;
using System.Collections.Generic;

namespace Chat.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Salt { get; set; }

    public string? Rol { get; set; }

    public string? Room { get; set; }

    public string? Avatar { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaUpdate { get; set; }

    public DateTime? LastConnection { get; set; }

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
