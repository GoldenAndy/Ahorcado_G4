using Ahorcado.Models;

namespace Ahorcado.Service
{
    public class AhorcadoService
    {
        private readonly Partida _juego = new Partida();

        public Partida Obtener()
        {
            return _juego;
        }

        public Partida NuevoJuego(string palabra)
        {
            _juego.Palabra = palabra.Trim().ToUpper();
            _juego.LetrasCorrectas = "";
            _juego.Fallos = 0;

            return _juego;
        }

        public Partida IntentarLetra(char letra)
        {
            letra = char.ToUpper(letra);

            if (_juego.Palabra.Contains(letra))
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
            string visible = "";

            foreach (char c in _juego.Palabra)
            {
                if (_juego.LetrasCorrectas.Contains(c))
                    visible += c;
                else
                    visible += "_";
            }

            return visible;
        }

        public bool Gano()
        {
            //Gana si adivina todas las letras
            foreach (char c in _juego.Palabra)
            {
                if (!_juego.LetrasCorrectas.Contains(c))
                    return false;
            }

            return true;
        }

        public bool Perdio()
        {
            return _juego.Fallos >= 6;
        }

        public string ObtenerImagen()
        {
            int f = _juego.Fallos;

            if (f > 6)
                f = 6;

            return $"/img/ahorcado_{f}.png";
        }
    }
}
