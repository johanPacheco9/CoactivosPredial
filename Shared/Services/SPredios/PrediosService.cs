using Dapper;
using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Models;
using Npgsql;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services
{
    public class PrediosService : IPrediosService
    {
        private readonly IConnectionService _connectionService;
        private readonly IConfiguration _configuration; // para usar dapper
        private readonly AppDbContext _dbContext; // para usar Entity Framework


        public PrediosService(IConfiguration configuration, AppDbContext dbContext, IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        
        
       public async Task<List<Predios>> GetAll()
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Predios> comparendos = new List<Predios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();

                    var query = "SELECT c.*,x.apellidos||' '||x.nombre as nombre_inf,a.nombre as nagente FROM comparendos c inner join infractores x on (x.cedula=c.cedula and x.tipo_ident=c.tipo_id) left join agentes a on (a.placa=c.agente) limit 10";
                    comparendos = con.Query<Predios>(query).ToList();
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine("Ocurrió un error al obtener los comparendos: " + ex.Message);

                    throw new Exception("Error al obtener los comparendos", ex);
                }
                finally
                {
                    con.Close();
                }
            }

            return comparendos;
        }

        public async Task<List<ComparendoInfo>> GetInfoComparendo(string pcedula)
        {
            
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<ComparendoInfo> resoluciones = new List<ComparendoInfo>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "SELECT codigo,avaluo,dir,matricula,s_nulo(nombre)  as nombre FROM maestro WHERE cedula like  '" + pcedula + "%'";
                    resoluciones = (await con.QueryAsync<ComparendoInfo>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                return resoluciones;
            }
        }


       public async Task<Predios> GetMultasByComp(string comp)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            Predios comparendosList = new Predios();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT c.*,x.nombre as ntipo FROM maestro c left join tipos_ident x on (x.id=c.tipo) " +
                        "WHERE c.codigo = '" + comp + "'";
                    comparendosList = await con.QueryFirstOrDefaultAsync<Predios>(query);
                }
                catch (Exception ex)
                {
                 Console.WriteLine($"Error en consulta Liquidacion: {ex.Message}");
                }
                finally
                {
                    con.Close();
                }
            }
            return comparendosList;

        }

        public async Task<List<Predios>> GetComparendosByCedula(string cedula)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Predios> comparendosList = new List<Predios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT c.*,s_nulo(x.apellidos)||' '||s_nulo(x.nombre) as nombre_inf FROM comparendos c inner join infractores x on " +
                        "(x.cedula=c.cedula and x.tipo_ident=c.tipo_id) where c.cedula like '" + cedula + "%' limit 15";
                    comparendosList = con.Query<Predios>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }

            return comparendosList;
        }


        public async Task<List<Documentos>> GetPdfsByComp(string comp)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Documentos> comparendosList = new List<Documentos>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "select * from muestra_pdfs('" + comp + "')";
                    comparendosList = con.Query<Documentos>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }

            return comparendosList;
        }

        public async Task<int> DeleteDocs(int id)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            var count = 0;

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "DELETE FROM documentos WHERE id =" + id;
                    count = con.Execute(query);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                return count;
            }
        }
        public async Task<int> Add_Docs(Documentos docs_obj)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "INSERT INTO documentos(codigo_predial,nombre,fecha,archivo) values (@codigo_predial,@nombre,@fecha,@archivo);";
                    count = con.Execute(query, docs_obj);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                return count;
            }
        }

        public async Task<List<Procesos>> GetCoactivo(string comp)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Procesos> comparendosList = new List<Procesos>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT p.*,e.nombre as nestado FROM procesos p inner join estados_proc e on (e.id=p.estado) WHERE p.comparendo = '" + comp + "'";
                    comparendosList = con.Query<Procesos>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }

            return comparendosList;
        }

    }
}

