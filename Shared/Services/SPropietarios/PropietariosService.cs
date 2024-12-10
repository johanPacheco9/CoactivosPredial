using Dapper;
using Microsoft.Extensions.Configuration;
using Coactivos_Predial.Models;
using Npgsql;

using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Shared.Services.SPropietarios;
using Coactivos_Predial.Shared.Services.SDatabase;
using System.Security.Claims;

namespace Coactivos_Predialp.Shared.Services.SPropietarios
{
    public class PropietariosService : IpropietariosService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConnectionService _connectionService;
        private readonly IConfiguration _configuration; // para usar dapper
        private readonly AppDbContext _dbContext; // para usar Entity Framework

        public PropietariosService(IConfiguration configuration, AppDbContext dbContext, IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _configuration = configuration;
            _dbContext = dbContext;
            
        }


                
        public async Task<int> Add(Propietarios infractores)
        
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "INSERT INTO infractores(cedula,tipo_ident,nombre,direccion,tel,licencia,ciudad,correo,apellidos,categoria,ciudad_lic) VALUES(@cedula,@tipo_ident,@nombre,@direccion,@tel,@licencia,@ciudad,@correo,@apellidos,@categoria,@ciudad_lic);";
                    count = con.Execute(query, infractores);
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

        public int DeleteInfractores(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> EditInfractores(Propietarios infractores)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            var count = 0;

            using (var con = new NpgsqlConnection(connectionString))
            {

                try
                {
                    con.Open();
                    var query = "UPDATE infractores SET cedula=@cedula,tipo_ident=@tipo_ident,nombre=@nombre,direccion=@direccion,tel=@tel,licencia=@licencia,ciudad=@ciudad,correo=@correo,apellidos=@apellidos,categoria=@categoria,ciudad_lic=@ciudad_lic WHERE id = @id";
                    count = con.Execute(query, infractores);
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

        public Propietarios GetInfractores(int id)
        {
            throw new NotImplementedException();
        }

        
        public async Task<List<Propietarios>> GetList()
        {

            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");

            List<Propietarios> infractores = new List<Propietarios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();

                    var query = "SELECT * FROM \"infractores\" limit 10";
                    infractores = con.Query<Propietarios>(query).ToList();
                }
                catch (NpgsqlException ex)
                {
                    // Aquí puedes realizar acciones adicionales, como registrar el error en un archivo de registro
                    // o mostrar un mensaje de error más descriptivo al usuario.
                    Console.WriteLine("Ocurrió un error al obtener los comparendos: " + ex.Message);

                    // También puedes lanzar una nueva excepción personalizada si lo deseas.
                    throw new Exception("Error al obtener los comparendos", ex);
                }
                finally
                {
                    con.Close();
                }
            }

            return infractores;
        }

        public async Task<string> GetNameInfractorBycode(string code,int tipo)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();

                    var query = "SELECT apellidos||' '||nombre as nombre FROM \"infractores\" WHERE cedula = @codigo and tipo_ident= @xtipo";

                    var nombreInfractor = await con.QueryFirstOrDefaultAsync<string>(query, new { codigo = code , xtipo=tipo});

                    // Devolver el nombre de la infracción
                    return nombreInfractor;
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
        }


        public async Task<List<Propietarios>> GetAll()
        {


            
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");

            List<Propietarios> infractor = new List<Propietarios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();

                    var query = "SELECT * FROM propietarios limit 10";
                    infractor = con.Query<Propietarios>(query).ToList();
                }
                catch (NpgsqlException ex)
                {
                    // Aquí puedes realizar acciones adicionales, como registrar el error en un archivo de registro
                    // o mostrar un mensaje de error más descriptivo al usuario.
                    Console.WriteLine("Ocurrió un error al obtener los comparendos: " + ex.Message);

                    // También puedes lanzar una nueva excepción personalizada si lo deseas.
                    throw new Exception("Error al obtener los comparendos", ex);
                }
                finally
                {
                    con.Close();
                }
            }

            return infractor;
        }

        public async Task<Propietarios> GetInfractorById(int id)
        {
            try
            {
                var infractor = await _dbContext.FindAsync<Propietarios>(id);

                if (infractor == null)
                {
                    Console.WriteLine($"No se encontró un infractor con id {id}");
                    return null;
                }

                // Handle null value for 'correo' column
           //     infractor.correo = DBNull.Value.Equals(infractor.correo) ? string.Empty : (string)infractor.correo;

                return infractor;
            }
            catch (Exception ex)
            {
                // Loguear la excepción o manejarla de otra manera apropiada
                throw new Exception("Error al obtener infractor por ID", ex);
            }
        }

        //GetInfractoresByCedula(string cedula);
        public async Task<List<Propietarios>> GetInfractoresByCedula(string cedula)
        {

            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Propietarios> infractoresList = new List<Propietarios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM propietarios WHERE documento like '" + cedula + "%' limit 20";
                    infractoresList = con.Query<Propietarios>(query).ToList();
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

            return infractoresList;
        }

        public async Task<List<Propietarios>> PropietariosBypredial(string codigo)
        {
            var connectionString = _connectionService.GetConnectionString("DefaultConnectionTemplate");
            List<Propietarios> infractoresList = new List<Propietarios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "select x.documento,p.tipo_id,x.nombre,t.nombre as ntipo from predios_prop p inner join propietarios x on "+
                      " (x.documento=p.cedula and x.tipo_id=p.tipo_id) inner join tipos_ident t on (t.id=x.tipo_id) where p.no_predial = '" + codigo + "' ";
                    infractoresList = con.Query<Propietarios>(query).ToList();
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

            return infractoresList;
        }


    }



}

