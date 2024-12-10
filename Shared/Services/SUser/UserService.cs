using Dapper;
using Coactivos_Predial.Models;

using Coactivos_Predial.Shared.Models;
using Npgsql;
using System.Numerics;
using System.Security.Cryptography;
using Coactivos_Predial.Pages.PLogin;
using Coactivos_Predial.Shared.Services.SLogin;
using Coactivos_Predial.Shared.Services.SDatabase;


namespace Coactivos_Predial.Shared.Services.SUser
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration; // para usar dapper


        public UserService(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
        }

        public string GetConnection()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Add(Usuarios usuarios)
        {
            var connectionString = this.GetConnection();
            int count = 0;
            using (var con = new NpgsqlConnection(connectionString))
            {
                usuarios.clave = PasswordHash("123");
                usuarios.usuario_creo = 3;
                try
                {
                    await con.OpenAsync();
                    var query = "INSERT INTO usuarios(nombre,dir,tel,login,clave,nivel,correo,habilitado,municipio,mun) VALUES(@nombre,@dir,@tel,@login,@clave,@nivel,@correo,@habilitado,@municipio,@mun);";
                    count = (await con.ExecuteAsync(query, usuarios));

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


        public bool ValidatePasswordHash(string password, string dbPassword)
        {
            byte[] dbPasswordHashBytes = Convert.FromBase64String(dbPassword);

            byte[] salt = new byte[16];
            Array.Copy(dbPasswordHashBytes, 0, salt, 0, 16);

            var userPasswordBytes = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] userPasswordHash = userPasswordBytes.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (dbPasswordHashBytes[i + 16] != userPasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<int> DeleteUsuarioById(int id)
        {

            var connectionString = this.GetConnection();
            var count = 0;

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "DELETE FROM usuarios WHERE id =" + id;
                    count = ( await con.ExecuteAsync(query));
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
        public async Task<int> EditUsuarios(Usuarios usuarios)
        {
            var connectionString = this.GetConnection();
            var count = 0;

            using (var con = new NpgsqlConnection(connectionString))
            {

                try
                {
                    await con.OpenAsync();
                    var query = "UPDATE usuarios SET nombre=@nombre,dir=@dir,tel=@tel,login=@login,nivel=@nivel,correo=@correo,habilitado=@habilitado,municipio=@municipio,mun=@mun WHERE id = @id";
                    count =((await con.ExecuteAsync(query, usuarios)));
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




        public async Task<List<Usuarios>> GetAll()
        {
            var connectionString = this.GetConnection();
            List<Usuarios> usuarios = new List<Usuarios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM usuarios";
                    usuarios = (await con.QueryAsync<Usuarios>(query)).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                return usuarios;
            }
        }

        private string PasswordHash(string password)
        {
            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }
        public async Task<int> Reset_key(int pid)
        {
            var connectionString = this.GetConnection();
            var count = 0;
            string pclave= PasswordHash("123");
            using (var con = new NpgsqlConnection(connectionString))
            {

                try
                {
                    await con.OpenAsync();
                    var query = "UPDATE usuarios SET clave='"+pclave +"' WHERE id = '"+pid+"'";
                    count = ((await con.ExecuteAsync(query)));
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

        public async Task<int> Change_key(int pid,string skey)
        {
            var connectionString = this.GetConnection();
            var count = 0;
            string pclave = PasswordHash(skey);
            using (var con = new NpgsqlConnection(connectionString))
            {

                try
                {
                    await con.OpenAsync();
                    var query = "UPDATE usuarios SET clave='" + pclave + "' WHERE id = '" + pid + "'";
                    count = ((await con.ExecuteAsync(query)));
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
        public async Task<Usuarios> GetUserById(int id)
        {
            var connectionString = this.GetConnection();
            Usuarios usuarios = new Usuarios();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM usuarios WHERE id =" + id;
                    usuarios = (await con.QueryAsync<Usuarios>(query)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
                return usuarios;
            }
        }
        
        public async Task<List<Usuarios>> GetUserByName(string nombre)
        {
            var connectionString = this.GetConnection(); 
            List<Usuarios> usuariosList = new List<Usuarios>();

            using (var con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await con.OpenAsync();
                    var query = "SELECT * FROM \"usuarios\" WHERE nombre = @nombre";
                    usuariosList = con.Query<Usuarios>(query, new { nombre }).ToList();
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

            return usuariosList;
        }


    }
}
