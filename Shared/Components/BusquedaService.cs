using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Coactivos_Predial.Shared.Components.ComponentServices.BusquedaService
{
    public class BusquedaService
    {
        public string ultimoComparendoCaptured;
        public async Task<string> ComparendoCapturado(string comparendo)
        {
            ultimoComparendoCaptured = comparendo;
            Console.WriteLine($"comparendo desde el servicio: {ultimoComparendoCaptured}");
            return ultimoComparendoCaptured;
        }

        public string ObtenerUltimoComparendoCapturado()
        {
            return ultimoComparendoCaptured;
        }
    }

}
