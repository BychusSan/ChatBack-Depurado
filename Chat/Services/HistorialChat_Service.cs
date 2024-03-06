using Chat.Models;

namespace Chat.Services
{
    public class HistorialChat_Service
    {
        private readonly ChatContext _dbContext;

        public HistorialChat_Service(ChatContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Mensaje> GetHistorialChat(string sala)
        {
            return _dbContext.Mensajes.Where(x => x.Rol == sala).OrderBy(z => z.FechaMensaje).ToList();
        }


        public List<string> GetSalasDisponibles()
        {
            return _dbContext.Mensajes.Select(x => x.Rol).Distinct().ToList();
        }

    }
}
