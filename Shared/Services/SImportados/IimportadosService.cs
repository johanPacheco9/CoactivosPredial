using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SImportados
{
    public interface IimportadosService
    {
        Task<int> Add(Importados import_obj);

        Task<int> Addrec(Recaudo_Externo import_rec);
        Task<List<Importados>> GetList();
        
        Task<List<Recaudo_Externo>> GetListRec();

        Task<int> DeleteAll();
        Task<int> DeleteRec();
        Task<int> Procesa_Comp(int puser);
        
        Task<int> Procesa_Rec(int puser);
    }
}
