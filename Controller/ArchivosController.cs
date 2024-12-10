using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Coactivos_Predial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly IConnectionService _connectionService;
        private readonly string _archivosFolder;

        public ArchivosController(IConnectionService connectionService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
            _archivosFolder = _connectionService.GetStorageString("FileStorageTemplate");
        }

        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_archivosFolder, fileName);

                Console.WriteLine($"el filepath en el controller es : {filePath}");
                if (System.IO.File.Exists(filePath))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;

                    Console.WriteLine($"Archivo {fileName} leído correctamente.");

                    // Devolver el archivo como un FileResult para que el navegador lo descargue
                    return File(memory, "application/pdf", fileName);
                }
                else
                {
                    Console.WriteLine($"El archivo {fileName} no existe en el servidor.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
