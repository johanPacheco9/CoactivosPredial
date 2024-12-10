namespace Coactivos_Predial.Shared.Services.UserAccesor
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserMunicipio()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("Municipio")?.Value;
        }
    }
}
