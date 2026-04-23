namespace BlazorServerCPP.Models
{
    public class Permesso
    {
        public string Nome { get; set; }
        public Dictionary<string, bool> Ruoli { get; set; } = new();
    }
}
