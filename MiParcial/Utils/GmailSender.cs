using System.Net.Mail;
using System.Net;
using System.Text;

namespace MiParcial.Utils
{
    public static class GmailSender
    {
        private static string correoEmisor = "+";
        private static string appEmailPassword = "+";

        /// <summary>
        /// Metodo para Enviar un Correo Electronico
        /// </summary>
        /// <param name="correoDestinatario"> Email de Destino a enviar.</param>
        /// <param name="asunto"> Asunto o Encabezado del Correo</param>
        /// <param name="cuerpoMensaje"> Mensaje que se desea enviar por Email.</param>        /// 
        public static bool SendEmail(string correoDestinatario, string asunto, string cuerpoMensaje)
        {
            string from = correoEmisor;
            string to = correoDestinatario;
            string subject = asunto;
            string body = cuerpoMensaje;

            using (MailMessage mail = new MailMessage(from, to))
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Puedes establecer esto a true si deseas enviar un correo HTML

                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    
                    client.Port = 587;
                    client.Credentials = new NetworkCredential(correoEmisor, appEmailPassword);
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(mail);
                        Console.WriteLine("Correo enviado con éxito");
                        return true; //enviado con exito.

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                        return false; //error al enviar.
                    }
                }
            }
        }

    }
}
