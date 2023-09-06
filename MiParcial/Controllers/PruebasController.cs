using Microsoft.AspNetCore.Mvc;
using MiParcial.Utils;

namespace MiParcial.Controllers
{
    //Este es un controller utilizado unicamente para realizar pruebas.
    public class PruebasController : Controller
    {
        public IActionResult Index()
        {            
            return View();
        }

        public IActionResult EnviarCorreo()
        {
            string correoDestinatario = "frankexe2019@gmail.com";
            string asuntoCorreo = "prueba";
            string cuerpoCorreo = "pruebaaa";

            bool enviadoExitosamente = GmailSender.SendEmail(correoDestinatario, asuntoCorreo, cuerpoCorreo);
            if (enviadoExitosamente)
            {
                TempData["SuccessMessage"] = "Correo enviado con éxito.";
            }
            else
            {
                //ERROR
                ModelState.AddModelError(string.Empty, "Error al enviar correo.");
            }


            return RedirectToAction("Resultado");
        }

        public IActionResult Resultado()
        {
            // Enviar mensajes para renderizar en la vista
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View();
        }
    }
}
