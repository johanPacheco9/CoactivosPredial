using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services
{
    public interface IPrediosService
    {
              
        Task<List<Predios>> GetComparendosByCedula(string cedula);
        Task<List<Predios>> GetAll();
        Task<int> Add_Docs(Documentos docs_obj);

        Task<int> DeleteDocs(int id);
        Task<List<ComparendoInfo>> GetInfoComparendo(string pcedula);
        Task<Predios> GetMultasByComp(string comp);

        Task<List<Documentos>> GetPdfsByComp(string comp);

        Task<List<Procesos>> GetCoactivo(string comp);

    }   
}