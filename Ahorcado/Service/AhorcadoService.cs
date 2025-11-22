using System.Text.Json;
using System.Text.Json.Serialization;
using Ahorcado.Models;

namespace Ahorcado.Service
{
    public class AhorcadoService
    {
        private readonly Partida _juego = new Partida();
        private readonly HttpClient _httpClient;

        private const int INTENTOS_MAXIMOS = 7;
        private const int LONGITUD_MINIMA = 6;
        private const int TOTAL_IMAGENES = 8;

        private static readonly string[] _fallbackPalabras =
        {
            "PROGRAMACION",
            "BATMAN",
            "TRANSPORTE",
            "AMARILLO",
            "LECTURA",
            "FUNCIONAMIENTO",
            "BIBLIOTECA",
            "HISTORIAS"
        };

        private class RandomWordApiResponse
        {
            [JsonPropertyName("word")]
            public string Word { get; set; } = "";
        }


        public AhorcadoService()
        {
            _httpClient = new HttpClient();
        }

        public Partida Obtener() => _juego;

        public Partida NuevoJuego(string palabra)
        {
            _juego.Palabra = (palabra ?? "").Trim().ToUpper();
            _juego.LetrasCorrectas = "";
            _juego.Fallos = 0;
            return _juego;
        }

        public async Task<Partida> NuevoJuegoDesdeApiAsync()
        {
            const int MAX_REINTENTOS = 10;


            for (int intento = 0; intento < MAX_REINTENTOS; intento++)
            {
                try
                {
                    var response = await _httpClient.GetAsync("https://random-words-api.vercel.app/word/spanish");
                    if (!response.IsSuccessStatusCode)
                        continue;

                    var json = await response.Content.ReadAsStringAsync();
                    var opciones = JsonSerializer.Deserialize<List<RandomWordApiResponse>>(json);
                    var palabra = opciones?.FirstOrDefault()?.Word;

                    if (!string.IsNullOrWhiteSpace(palabra))
                    {
                        palabra = palabra.ToUpper().Trim();

                        if (palabra.Length >= LONGITUD_MINIMA && palabra.All(c => char.IsLetter(c)))
                        {
                            _juego.Palabra = palabra;
                            _juego.LetrasCorrectas = "";
                            _juego.Fallos = 0;
                            return _juego;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }


            var rnd = new Random();
            var fallback = _fallbackPalabras[rnd.Next(_fallbackPalabras.Length)];
            if (string.IsNullOrWhiteSpace(fallback))
                fallback = "PROGRAMACION";

            _juego.Palabra = fallback.ToUpper();
            _juego.LetrasCorrectas = "";
            _juego.Fallos = 0;

            return _juego;
        }

        public Partida IntentarLetra(char letra)
        {
            letra = char.ToUpper(letra);

            if (!string.IsNullOrEmpty(_juego.Palabra) && _juego.Palabra.Contains(letra))
            {
                if (!_juego.LetrasCorrectas.Contains(letra))
                    _juego.LetrasCorrectas += letra;
            }
            else
            {
                _juego.Fallos++;
            }

            return _juego;
        }

        public string ObtenerEstadoVisible()
        {
            if (string.IsNullOrEmpty(_juego.Palabra))
                return "";

            string visible = "";

            foreach (char c in _juego.Palabra)
            {
                visible += _juego.LetrasCorrectas.Contains(c) ? c : "_";
            }

            return visible;
        }

        public bool Gano()
        {

            if (string.IsNullOrEmpty(_juego.Palabra))
                return false;

            foreach (char c in _juego.Palabra)
            {
                if (!_juego.LetrasCorrectas.Contains(c))
                    return false;
            }
            return true;
        }

        public bool Perdio() => _juego.Fallos >= INTENTOS_MAXIMOS;

        public string ObtenerImagen()
        {


            int f = _juego.Fallos + 1;

            if (f < 1) f = 1;
            if (f > TOTAL_IMAGENES) f = TOTAL_IMAGENES;

            return $"/RecursosVisuales/ahorcado{f}.jpg";
        }

    }
}
