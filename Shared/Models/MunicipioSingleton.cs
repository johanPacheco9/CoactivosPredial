namespace Coactivos_Predial.Shared.Models
{
    public static class MunicipioSingleton
    {
        private static string _municipio;

        public static string Municipio
        {
            get => _municipio;
            set => _municipio = value;
        }
    }
}
