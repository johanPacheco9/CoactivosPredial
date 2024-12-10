using Dapper;
using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Npgsql;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SParametros
{
    public class ParametroService : IParametroService
    {
        private readonly IConnectionService _connectionService;
        private readonly IConfiguration _configuration; // para usar dapper
        private readonly AppDbContext _dbContext; // para usar Entity Framework
        
        public ParametroService(IConfiguration configuration, AppDbContext dbContext, IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _configuration = configuration;
            _dbContext = dbContext;
        }


        public async Task<int> EditParametros(int id, Parametros parametros)
        {

            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            var count = 0;

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync(); // Utilizar OpenAsync para operaciones asincrónicas
                    var query = "UPDATE parametros SET dir = @dir, ciudad = @ciudad, nit = @nit, tel = @tel, vlr_recibo = @vlr_recibo, cuotas_finan=@cuotas_finan,porc_min_fin=@porc_min_fin,fin_nueva_ley=@fin_nueva_ley,cobra_adicional=@cobra_adicional," +
                                "nombre=@nombre,tipo_acuerdo=@tipo_acuerdo,tipo_inicial=@tipo_inicial,divipo=@divipo,fec_ini_interes=@fec_ini_interes,dias_persuasivo=@dias_persuasivo,dh_fecha6=@dh_fecha6 WHERE id = @id";
                    count = await con.ExecuteAsync(query, parametros); // Utilizar ExecuteAsync para operaciones asincrónicas
                }
                catch (Exception ex)
                {
                    // Considerar el manejo adecuado de la excepción (registro, notificación, etc.)
                    throw new Exception("Error al editar los parámetros", ex);
                }
            }

            return count;
        }

        public async Task<List<Parametros>> GetAll()
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Parametros> parametros = new List<Parametros>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM \"Parametros\"";
                    parametros = con.Query<Parametros>(query).ToList();
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
            return parametros;
        }

        public async Task<int> EditEstados(Estados_proc agentes_obj)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            var count = 0;

            using (var con = new NpgsqlConnection(connectionString))
            {

                try
                {
                    con.Open();
                    var query = "UPDATE estados_proc SET nombre=@nombre,porc=@porc WHERE id = @id";
                    count = con.Execute(query, agentes_obj);
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


       public async Task<List<Estados_proc>> Getlist()
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Estados_proc> agentes_obj = new List<Estados_proc>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();

                    var query = "SELECT * FROM \"estados_proc\" limit 10";
                    agentes_obj = con.Query<Estados_proc>(query).ToList();
                }
                catch (NpgsqlException ex)
                {
                    throw new Exception("Error al obtener los comparendos", ex);
                }
                finally
                {
                    con.Close();
                }
            }

            return agentes_obj;
        }

        public async Task<Parametros> GetParametroById(int id)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            Parametros parametros = null;

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM parametros WHERE id =1";
                    parametros = await con.QueryFirstOrDefaultAsync<Parametros>(query);
                }
                catch (Exception ex)
                {

                    throw new Exception("Error al obtener los parámetros por ID", ex);
                }
            }

            return parametros;
        }

    }
}
