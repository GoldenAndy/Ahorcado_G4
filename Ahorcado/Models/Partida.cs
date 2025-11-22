namespace Ahorcado.Models
{
    public class Partida
    {
        public string Palabra { get; set; } = "";
        public string LetrasCorrectas { get; set; } = "";
        public int Fallos { get; set; } = 0;
    }
}
