namespace Andloe.Entidad
{
    public sealed class AlanubeConfigDto
    {
        public string BaseUrl { get; set; } = "";
        public string Token { get; set; } = "";
        public string Ambiente { get; set; } = "sandbox";
        public int TimeoutSegundos { get; set; } = 60;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(BaseUrl) &&
            !string.IsNullOrWhiteSpace(Token);
    }
}