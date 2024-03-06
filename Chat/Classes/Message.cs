public class Message

{
    public string User { get; set; }
    public string Text { get; set; }
    public string Avatar { get; set; }
    public string Room { get; set; }

    public int? Id { get; set; }    
    public DateTime Fecha { get; set; }
    public string? Receptor { get; set; }
}
