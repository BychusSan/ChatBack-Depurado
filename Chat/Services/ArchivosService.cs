using Chat.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace Chat.Services
{
    public class ArchivosService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArchivosService(ChatContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<string>> GetTodosLosArchivos()
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                return new List<string>();
            }

            var archivosEnCarpeta = Directory.GetFiles(uploadsFolder);

            return archivosEnCarpeta.Select(Path.GetFileName).ToList();
        }

        public async Task<List<string>> GetArchivosPorTipo3(string tipo)
        {
            string uploadsFolder = ObtenerRutaUploads();

            if (!Directory.Exists(uploadsFolder))
            {
                return new List<string>();
            }

            var archivosFiltradosInicio = Directory.GetFiles(uploadsFolder)
          .Where(file => ObtenerTipoContenido(file)?.StartsWith(tipo, StringComparison.OrdinalIgnoreCase) ?? false);

            var archivosFiltradosFin = Directory.GetFiles(uploadsFolder)
                .Where(file => ObtenerTipoContenido(file)?.EndsWith(tipo, StringComparison.OrdinalIgnoreCase) ?? false);

            var archivosFiltrados = archivosFiltradosInicio.Concat(archivosFiltradosFin);


            return archivosFiltrados.Select(Path.GetFileName).ToList();
        }


        public string ObtenerTipoContenido(string nombreArchivo)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string filePath = Path.Combine(uploadsFolder, nombreArchivo);

            var tipoContenido = new FileExtensionContentTypeProvider();

            if (tipoContenido.TryGetContentType(filePath, out var contentType))
            {
                return contentType;
            }

            return null;
        }

        private string ObtenerRutaUploads()
        {
            return Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        }
    }
}