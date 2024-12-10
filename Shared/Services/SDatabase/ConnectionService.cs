using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;

public class ConnectionService : IConnectionService
{
    private readonly IConfiguration _configuration;

    public ConnectionService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString(string municipio)
    {
        try
        {
            municipio = MunicipioSingleton.Municipio;

            if (string.IsNullOrEmpty(municipio))
            {
                throw new InvalidOperationException("El municipio no está establecido.");

            }

            var connectionStringTemplate = _configuration.GetConnectionString("DefaultConnectionTemplate");

            // Reemplazar el marcador {Database} con el nombre del municipio
            return connectionStringTemplate.Replace("{Database}", municipio);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public string GetStorageString(string ubicacion)
    {
        try
        {
            ubicacion = MunicipioSingleton.Municipio.Trim();
            if (string.IsNullOrEmpty(ubicacion))
            {
                throw new InvalidOperationException("La ruta no está establecida.");
            }

            var storageStringTemplate = _configuration["FileStorageTemplate"];

            // Reemplazar el marcador {Predial} con el valor dinámico
            return storageStringTemplate.Replace("{Predial}", ubicacion);
        }
        catch (Exception ex)
        {
            // Manejar el error según sea necesario
            throw new InvalidOperationException("Error al obtener la cadena de conexión de almacenamiento", ex);
        }
    }
}
