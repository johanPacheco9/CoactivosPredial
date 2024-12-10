using Dapper;
using Coactivos_Predial.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.IO.Pipelines;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Coactivos_Predial.Shared.Services.Spersuasivo
{
    public class PersuasivoService :IPersuasivoService
    {

        private readonly IConfiguration _configuration; // para usar dapper
        private readonly AppDbContext _dbContext; // para usar Entity Framework

        public PersuasivoService(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }
        public string GetConnection()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Persuasivos>> GetList(int opcion,string pcomp,DateTime pdesde,DateTime phasta,int pvigencia,int ptran)
        {
            var connectionString = this.GetConnection();
            List<Persuasivos> persuasivo = new List<Persuasivos>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM sin_persuasivo('" + opcion + "', '" + pcomp + "','" + pdesde + "','" + phasta + "','" + pvigencia + "','" + ptran+ "')";
                    persuasivo = (await con.QueryAsync<Persuasivos>(query)).ToList();
                }
                catch (Exception ex)
                {

                    throw new Exception("no se encontraron coactivos");
                }

                return persuasivo;
            }
        }

        public async Task<int> Procesa_Per(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            try
            {
                var connectionString = GetConnection();
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.persuasivo(@opcion,@pcomp,@pdesde,@phasta,@pvigencia,@ptran)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.Add(new NpgsqlParameter("@opcion", NpgsqlDbType.Integer) { Value = opcion });
                cmd.Parameters.Add(new NpgsqlParameter("@pcomp", NpgsqlDbType.Text) { Value = pcomp });
                cmd.Parameters.Add(new NpgsqlParameter("@pdesde", NpgsqlDbType.Date) { Value = pdesde });
                cmd.Parameters.Add(new NpgsqlParameter("@phasta", NpgsqlDbType.Date) { Value = phasta });
                cmd.Parameters.Add(new NpgsqlParameter("@pvigencia", NpgsqlDbType.Integer) { Value = pvigencia });
                cmd.Parameters.Add(new NpgsqlParameter("@ptran", NpgsqlDbType.Integer) { Value = ptran });
                Console.WriteLine("Ejecutando persuasivos");
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                Console.WriteLine($"Error en Persuasivo: {ex.Message}");
                return 0; // Indicar que la operación falló
            }
        }

    }
}
