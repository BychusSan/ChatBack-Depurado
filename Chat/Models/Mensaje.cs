using System;
using System.Collections.Generic;

namespace Chat.Models;

public partial class Mensaje
{
    public int IdMensaje { get; set; }

    public string Nombre { get; set; } = null!;

    public string Texto { get; set; } = null!;

    public string? Rol { get; set; }

    public DateTime FechaMensaje { get; set; }

    public int? Id { get; set; }

    public virtual Usuario? IdNavigation { get; set; }
}
