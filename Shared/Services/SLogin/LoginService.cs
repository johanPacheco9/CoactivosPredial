using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;


namespace Coactivos_Predial.Shared.Services.SLogin
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _configuration; // para usar dapper

        

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly AppDbContext _dbContext;

        public string GetConnection()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        public LoginService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _configuration = configuration;
        }


        public async Task<(bool Success, string Message)> UserRegistrationAsync(Usuarios usuarios)
        {
            try
            {
                Console.WriteLine("Ejecutando Reistrar");
                Usuarios newUser = new();
                Console.WriteLine($"la clave del usuario en post es:{usuarios.clave}");
                newUser.login = usuarios.login;
                newUser.nombre = usuarios.nombre;
                newUser.clave = PasswordHash(usuarios.clave);
                _dbContext.usuarios.Add(newUser);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrarse:{ex}");
            }

            return (true, string.Empty);

        }


        //cambiar hash.
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

        public async Task<int> UserLoginAsync(string username, string pass)
        {
            // var connectionString = this.GetConnection();
            try
            {
                Console.WriteLine($"la contraseña en login es:{pass} y el user name es: {username}");
                 Usuarios user = await _dbContext.usuarios.Where(c => c.login.ToLower() == username).FirstOrDefaultAsync();

               
                Console.WriteLine($"paso x aca");
                if (user == null)
                {
                    Console.WriteLine($"Credenciales incorrectas");
                }
                if (pass == null)
                {
                    Console.WriteLine("Debe ingresasr contraseña");
                }
                if (!ValidatePasswordHash(pass, user.clave))
                {
                    Console.WriteLine($"Contraseña incorrecta:{0}");
                    return 0;
                }
                MunicipioSingleton.Municipio = user.mun;
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.nombre),
                        new Claim(ClaimTypes.Role, user.nivel.ToString()),
                        new Claim("Municipio", user.mun),
                        new Claim("UserId", user.id.ToString()),
                    };
                
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = true,
                    // Otras propiedades...
                };
                await _httpContextAccessor.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                Console.WriteLine($"usuario despues de la claim:{user.nombre}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Generar recibo: {ex.Message}");
                Console.WriteLine($"No pueden ir nulos los campos al iniciar sesión");
                return 0;
            }
            return 1;
        }
    }
}
