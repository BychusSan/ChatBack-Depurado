using Chat.DTOs;
using Chat.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;


namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly ArchivosService _archivosService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ArchivosController(ArchivosService archivosService, IWebHostEnvironment webHostEnvironment)
        {
            _archivosService = archivosService;
            _webHostEnvironment = webHostEnvironment;
        }

        #region GET
        [HttpGet("todosLosArchivos")]
        public IActionResult GetTodosLosArchivos()
        {
            try
            {
                var archivos = _archivosService.GetTodosLosArchivos();

                return Ok(archivos);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de archivos: {ex.Message}");
            }
        }


        [HttpGet("archivosPorTipo/{tipo}")]
        public async Task<IActionResult> GetArchivosPorTipo(string tipo)
        {
            try
            {
                var archivos = await _archivosService.GetArchivosPorTipo3(tipo);

                var archivosDto = archivos.Select(nombreArchivo => new DTOArchivosTipo
                {
                    Nombre = nombreArchivo,
                    //Url = $"https://localhost:7217/api/Archivos/descargarArchivo/{nombreArchivo}",
                    Url = $"http://gabrielsan-001-site1.ftempurl.com/api/Archivos/descargarArchivo/{nombreArchivo}",
                    TipoArchivo = _archivosService.ObtenerTipoContenido(nombreArchivo),
                    Extension = ObtenerExtensionArchivo(nombreArchivo)

                }).ToList();

                return Ok(archivosDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de archivos por tipo: {ex.Message}");
            }
        }



        private string ObtenerExtensionArchivo(string nombreArchivo)
        {
            return Path.GetExtension(nombreArchivo).TrimStart('.');
        }




        [HttpGet("descargarArchivo/{nombreArchivo}")]
        public IActionResult DescargarArchivo(string nombreArchivo)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", nombreArchivo);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"El archivo '{nombreArchivo}' no existe.");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                var contentType = _archivosService.ObtenerTipoContenido(nombreArchivo);
                return File(memory, contentType, nombreArchivo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al descargar el archivo: {ex.Message}");
            }
        }
        #endregion

        #region DELETE
        [HttpDelete("borrarArchivos")]
        public IActionResult BorrarArchivos()
        {
            try
            {
                string uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (Directory.Exists(uploadsFolderPath))
                {
                    Directory.Delete(uploadsFolderPath, true);
                    Directory.CreateDirectory(uploadsFolderPath); // Vuelve a crear la carpeta vacía
                    return Ok("Archivos eliminados correctamente.");
                }
                else
                {
                    return NotFound("La carpeta de archivos no existe.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al borrar archivos: {ex.Message}");
            }
        }
        [HttpDelete("borrarArchivo/{nombreArchivo}")]
        public IActionResult BorrarArchivo(string nombreArchivo)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", nombreArchivo);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    // Devolver un estado 204 (No Content) indicando que la operación fue exitosa
                    return NoContent();
                }
                else
                {
                    // Devolver un estado 404 (Not Found) si el archivo no existe
                    return NotFound($"El archivo '{nombreArchivo}' no existe.");
                }
            }
            catch (Exception ex)
            {
                // Devolver un estado 500 (Internal Server Error) en caso de error
                return StatusCode(500, $"Error al borrar el archivo '{nombreArchivo}': {ex.Message}");
            }
        }


        #endregion

    }
}