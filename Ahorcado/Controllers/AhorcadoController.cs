using Ahorcado.Service;
using Microsoft.AspNetCore.Mvc;

namespace Ahorcado.Controllers
{
    public class AhorcadoController : Controller
    {
        private readonly AhorcadoService _service;

        public AhorcadoController(AhorcadoService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult NuevoJuego(string palabra)
        {
            if (string.IsNullOrWhiteSpace(palabra))
            {
                return BadRequest("La palabra es obligatoria.");
            }

            var juego = _service.NuevoJuego(palabra);
            var estado = _service.ObtenerEstadoVisible();

            return Json(new
            {
                estado,
                longitud = juego.Palabra.Length,
                fallos = juego.Fallos,
                imagen = _service.ObtenerImagen(),
                gano = _service.Gano(),
                perdio = _service.Perdio()
            });
        }


        [HttpGet]
        public async Task<IActionResult> NuevoJuegoApi()
        {
            var juego = await _service.NuevoJuegoDesdeApiAsync();
            var estado = _service.ObtenerEstadoVisible();

            return Json(new
            {
                estado,
                longitud = juego.Palabra.Length,
                fallos = juego.Fallos,
                imagen = _service.ObtenerImagen(),
                gano = _service.Gano(),
                perdio = _service.Perdio()
            });
        }


        [HttpPost]
        public IActionResult Intentar(string letra)
        {
            if (string.IsNullOrWhiteSpace(letra))
            {
                return BadRequest("La letra es obligatoria.");
            }

            var juego = _service.IntentarLetra(letra[0]);
            var estado = _service.ObtenerEstadoVisible();

            return Json(new
            {
                estado,
                longitud = juego.Palabra.Length,
                fallos = juego.Fallos,
                imagen = _service.ObtenerImagen(),
                gano = _service.Gano(),
                perdio = _service.Perdio()
            });
        }
    }
}
