using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Coactivos_Predial.Shared.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NpgsqlTypes;
using System.Data;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SImportados
{
    public class ImportadosService : IimportadosService
    {
        private readonly IConnectionService _connectionService;
        private readonly IConfiguration _configuration; // para usar dapper
        private readonly AppDbContext _dbContext; // para usar Entity Framework

        public ImportadosService(IConfiguration configuration, AppDbContext dbContext, IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<List<Importados>> GetList()
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            
            List<Importados> importobj = new List<Importados>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM \"importados\" limit 20";
                    importobj = (await con.QueryAsync<Importados>(query)).ToList();
                }
                catch (Exception ex)
                {

                    throw new Exception("no se encontraron Registros");
                }

                return importobj;
            }
        }


        public async Task<List<Recaudo_Externo>> GetListRec()
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            
            
            List<Recaudo_Externo> importobj = new List<Recaudo_Externo>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM \"simit_rec\" limit 20";
                    importobj = (await con.QueryAsync<Recaudo_Externo>(query)).ToList();
                }
                catch (Exception ex)
                {

                    throw new Exception("no se encontraron Registros");
                }

                return importobj;
            }
        }

        public async Task<int> DeleteAll()
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "delete from importados; ";
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

                return 0;
            }
        }

        public async Task<int> DeleteRec()
        {

            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "delete from simit_rec; ";
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

                return 0;
            }
        }
        public async Task<int> Addrec(Recaudo_Externo import_rec)
        {

            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "INSERT INTO simit_rec (conc,fecha,hora,fecha_real,cod_canal,descrip,efectivo,cheque,recaudo," +
                        "comparendo,infractor,divipo,num_con,tipo_rec,otro,vlr_adicional) " +
                       "values (@conc,@fecha,@hora,@fecha_real,@cod_canal,@descrip,@efectivo,@cheque,@recaudo,@comparendo,@infractor,@divipo,@num_con,@tipo_rec,@otro,@vlr_adicional); ";
                    count = con.Execute(query, import_rec);

                }
                catch (Exception ex)
                {
                    // Manejar la excepción
                    Console.WriteLine($"Error en GenerarAcuerdo: {ex.Message}");
                    return 0; // Indicar que la operación falló
                }
                finally
                {
                    con.Close();
                }

                return count;
            }
        }
        public async Task<int> Procesa_Rec(int puser)
        {
            try
            {

                var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.procesa_rec(@puser)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add(new NpgsqlParameter("@puser", NpgsqlDbType.Integer) { Value = puser });
                Console.WriteLine("Ejecutando Crea_incumplidos");
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                Console.WriteLine($"Error en GenerarAcuerdo: {ex.Message}");
                return 0; // Indicar que la operación falló
            }
        }
    
    public async Task<int> Procesa_Comp(int puser)
        {
            try
            {
                var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.procesa_import(@puser)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add(new NpgsqlParameter("@puser", NpgsqlDbType.Integer) { Value = puser });
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                Console.WriteLine($"Error en Procesar Importados : {ex.Message}");
                return 0; // Indicar que la operación falló
            }
        }

    
    public async Task<int> Add(Importados import_obj)
        {
            
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "INSERT INTO importados (codigo,cedula,nombre,dir,area_tet,area_con,avaluo,tipo) " +
                      "VALUES (@codigo,@cedula,@nombre,@dir,@area_tet,@area_con,@avaluo,@tipo)";
                    count = con.Execute(query, import_obj);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creando importados: {ex.Message}");
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                return count;
            }
        }
    }
}
