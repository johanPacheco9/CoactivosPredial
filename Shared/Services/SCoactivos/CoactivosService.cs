using Dapper;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using Coactivos_Predial.Shared.Models;
using Npgsql;
using NpgsqlTypes;

namespace Coactivos_Predial.Shared.Services.SCoactivos
{
    public class CoactivosService : ICoactivosService
    {
        private readonly IConfiguration _configuration; // para usar dapper

        public CoactivosService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetConnection()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Coactivos>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            var connectionString = this.GetConnection();
            List<Coactivos> coactivo = new List<Coactivos>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM sin_Coactivo('" + opcion + "', '" + pcomp + "','" + pdesde + "','" + phasta + "','" + pvigencia + "','" + ptran + "')";
                    coactivo = (await con.QueryAsync<Coactivos>(query)).ToList();
                }
                catch (Exception ex)
                {

                    throw new Exception("no se encontraron coactivos");
                }

                return coactivo;
            }
        }

        public async Task<List<Coactivos>> GetList1(string x)
        {
            var connectionString = this.GetConnection();
            List<Coactivos> coactivos = new List<Coactivos>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM sin_coactivo()";
                    coactivos = (await con.QueryAsync<Coactivos>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }

                return coactivos;
            }
        }

        public async Task<int> Procesa_Coactivo(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            try
            {
                var connectionString = GetConnection();
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.mandamiento(@opcion,@pcomp,@pdesde,@phasta,@pvigencia,@ptran)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.Add(new NpgsqlParameter("@opcion", NpgsqlDbType.Integer) { Value = opcion });
                cmd.Parameters.Add(new NpgsqlParameter("@pcomp", NpgsqlDbType.Text) { Value = pcomp });
                cmd.Parameters.Add(new NpgsqlParameter("@pdesde", NpgsqlDbType.Date) { Value = pdesde });
                cmd.Parameters.Add(new NpgsqlParameter("@phasta", NpgsqlDbType.Date) { Value = phasta });
                cmd.Parameters.Add(new NpgsqlParameter("@pvigencia", NpgsqlDbType.Integer) { Value = pvigencia });
                cmd.Parameters.Add(new NpgsqlParameter("@ptran", NpgsqlDbType.Integer) { Value = ptran });
                Console.WriteLine("Ejecutando Mandamiento");
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                Console.WriteLine($"Error en Mandamiento: {ex.Message}");
                return 0; // Indicar que la operación falló
            }
        }


        public async Task<List<Transac_Report>> GetListtran(string ptipo, int pclase)
        {
            var connectionString = this.GetConnection();
            List<Transac_Report> resol_rep = new List<Transac_Report>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "";
                    if (pclase == 1)
                    {
                        query = "select p.num_proc as transaccion,cast(p.fecha_proc as date) as fecha,count(*) as No from procesos p " +
                        "inner join comparendos c on (c.comparendo=p.comparendo) where p.tipo='" + ptipo + "' and c.estado=1 group by p.num_proc,p.fecha_proc ";
                    }
                    else
                    {
                        query = "select p.num_proc as transaccion,cast(p.fecha_proc as date) as fecha,count(*) as No from procesos p " +
                        "inner join comparendos c on (c.comparendo=p.comparendo) where p.tipo='" + ptipo + "' group by p.num_proc,p.fecha_proc ";
                    }
                    resol_rep = (await con.QueryAsync<Transac_Report>(query)).ToList();
                }
                catch (Exception ex)
                {

                    throw new Exception("no se encontraron coactivos");
                }

                return resol_rep;
            }
        }

    }
}
