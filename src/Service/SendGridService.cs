using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OrderService.src.Service
{
    /// <summary>
    /// Servicio encargado de manejar el envio de correos electronicos a traves de SendGrid.
    /// Este servicio permite notificar al usuario sobre distintos estados del pedido,
    /// incluyendo creacion, actualizacion de estado y cancelacion.
    /// </summary>
    public class SendGridService
    {
        /// <summary>
        /// Clave API de SendGrid obtenida desde las variables de entorno.
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Constructor de la clase SendGridService.
        /// Inicializa la clave API de SendGrid desde las variables de entorno.
        /// </summary>
        /// <param name="configuration">
        /// Configuración de la aplicación (no utilizada directamente).
        /// </param>
        /// <exception>
        /// Se lanza si la API Key de SendGrid no está configurada.
        /// </exception>
        public SendGridService()
        {
            _apiKey = Environment.GetEnvironmentVariable("API_KEY_SENDGRID") ?? throw new InvalidOperationException("SendGrid APIKEY no encontrado.");
        }

        /// <summary>
        /// Envia un correo electronico al usuario confirmando la creacion de una nueva orden.
        /// </summary>
        /// <param name="toEmail">
        /// Correo electrónico del destinatario.
        /// </param>
        /// <param name="OrderNumber">
        /// Numero de la orden generada.
        /// </param>
        /// <param name="UserName">
        /// Nombre del usuario que realizo la orden.
        /// </param>
        /// <param name="TotalPrice">
        /// Monto total de la orden.
        /// </param>
        public async Task SendCreateOrderEmail(string toEmail, string OrderNumber, string UserName, double TotalPrice)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("order.service.censudex@gmail.com", "Censudex Order Service");
            var to = new EmailAddress(toEmail);

            var subject = $"Orden {OrderNumber} Ha sido recibida con exito";

            var plainTextContent = $"{UserName} Hemos recibido tu pedido!. Tu orden {OrderNumber} esta pendiente de procesamiento. Muchas Gracias por tu compra!";

            var htmlContent = $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9;'>
                        <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                            <h2 style='color: #2c3e50; margin-bottom: 20px; text-align: center;'>✅ Orden Confirmada</h2>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Estimado {UserName},
                            </p>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Hemos recibido tu orden <strong style='color: #3498db;'>#{OrderNumber}</strong> exitosamente.
                            </p>
                            
                            <div style='background-color: #f0f8ff; padding: 20px; border-left: 4px solid #3498db; margin: 20px 0;'>
                                <p style='margin: 0; font-size: 14px; color: #555;'>
                                    <strong>Estado:</strong> Pendiente de procesamiento
                                </p>
                                <p style='margin: 10px 0 0 0; font-size: 18px; color: #2c3e50;'>
                                    <strong>Total:</strong> <span style='color: #27ae60; font-size: 22px;'>${TotalPrice:N0}</span>
                                </p>
                            </div>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Te notificaremos por correo las actualizaciones de tu pedido.
                            </p>
                            <p><strong>¡Muchas Gracias Por Tu Compra!</strong>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            
                            <p style='font-size: 14px; color: #777; text-align: center; margin: 0;'>
                                <strong>Censudex</strong> 
                            </p>
                        </div>
                    </div>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Envía un correo electrónico notificando al usuario sobre un cambio en el estado de su pedido.
        /// Puede incluir información de seguimiento si el estado es "enviado".
        /// </summary>
        /// <param name="toEmail">Correo electrónico del destinatario.</param>
        /// <param name="OrderNumber">Número de la orden.</param>
        /// <param name="state">Nuevo estado de la orden.</param>
        /// <param name="TrackingNumber">Número de seguimiento del envío (opcional).</param>
        public async Task SendchangeStateEmail(string toEmail, string OrderNumber, string state, string? TrackingNumber)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("order.service.censudex@gmail.com", "Censudex Order Service");
            var to = new EmailAddress(toEmail);

            string? subject;
            string? plainTextContent;
            string? htmlContent;

            if (state.ToLower() == "enviado")
            {
                subject = $"Tu pedido {OrderNumber} ha sido enviado";

                plainTextContent = $"Tu pedido ha sido enviado y esta en camino!. Numero de seguimiento: {TrackingNumber}. Transportista: https://www.blue.cl/";

                htmlContent = $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9;'>
                        <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                            <h2 style='color: #2c3e50; margin-bottom: 20px; text-align: center;'>📦 Tu orden ha sido enviada</h2>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Estimado,
                            </p>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                ¡Buenas noticias! Tu pedido <strong style='color: #3498db;'>#{OrderNumber}</strong> ha sido enviado y está en camino.
                            </p>
                            
                            <div style='background-color: #f0f8ff; padding: 20px; border-left: 4px solid #3498db; margin: 20px 0;'>
                                <p style='margin: 0 0 15px 0; font-size: 14px; color: #555;'>
                                    <strong>Información de seguimiento:</strong>
                                </p>
                                <p style='margin: 10px 0 0 0; font-size: 16px; color: #2c3e50;'>
                                    <strong>Número de seguimiento:</strong><br>
                                    <span style='color: #3498db; font-size: 18px;'>{TrackingNumber}</span>
                                </p>
                                <p style='margin: 15px 0 0 0; font-size: 16px; color: #2c3e50;'>
                                    <strong>Transportista:</strong><br>
                                    <a href='https://www.blue.cl/' style='color: #3498db; font-size: 18px; text-decoration: none;'>https://www.blue.cl/</a>
                                </p>
                            </div>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Puedes usar el número de seguimiento para rastrear tu pedido.
                            </p>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            
                            <p style='font-size: 14px; color: #777; text-align: center; margin: 0;'>
                                <strong>Censudex</strong> 
                            </p>
                        </div>
                    </div>";
            }
            else
            {
                subject = $"Tenemos noticias sobre tu pedido {OrderNumber}";

                plainTextContent = $"Tenemos noticias de tu pedido!. Tu pedido ahpra esta {state}";

                htmlContent = $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9;'>
                        <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                            <h2 style='color: #2c3e50; margin-bottom: 20px; text-align: center;'>☑️ Tu orden ha sido actualizada</h2>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Estimado,
                            </p>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Tenemos noticias sobre tu orden <strong style='color: #3498db;'>#{OrderNumber}</strong>
                            </p>
                            
                            <div style='background-color: #f0f8ff; padding: 20px; border-left: 4px solid #3498db; margin: 20px 0;'>
                                <p style='margin: 0; font-size: 14px; color: #555;'>
                                    El estado de tu orden ha sido actualizado
                                </p>
                                <p style='margin: 10px 0 0 0; font-size: 18px; color: #2c3e50;'>
                                    <strong>Estado:</strong> <span style='color: #3498db; font-size: 22px;'>{state}</span>
                                </p>
                            </div>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Te seguiremos notificando sobre cualquier cambio en tu pedido.
                            </p>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            
                            <p style='font-size: 14px; color: #777; text-align: center; margin: 0;'>
                                <strong>Censudex</strong> 
                            </p>
                        </div>
                    </div>";
            }

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Envia un correo electrónico notificando al usuario que su pedido ha sido cancelado.
        /// </summary>
        /// <param name="toEmail">
        /// Correo electrónico del destinatario.
        /// </param>
        /// <param name="OrderNumber">
        /// Numero de la orden cancelada.
        /// </param>
        /// <param name="reason">
        /// Motivo de la cancelación (si no se proporciona, se asigna un valor por defecto).
        /// </param>
        public async Task SendCancelOrderEmail(string toEmail, string OrderNumber, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                reason = "Sin razon proporcionada.";
            }

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("order.service.censudex@gmail.com", "Censudex Order Service");
            var to = new EmailAddress(toEmail);

            var subject = $"Orden {OrderNumber} Cancelada";

            var plainTextContent = $"Malas noticias, tu pedido {OrderNumber} fue cancelado. el motivo fue: {reason}";

            var htmlContent = $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9;'>
                        <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                            <h2 style='color: #2c3e50; margin-bottom: 20px; text-align: center;'>❌Orden Cancelada</h2>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Estimado,
                            </p>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Lamentamos informarte que tu pedido <strong style='color: #3498db;'>#{OrderNumber}</strong> fue cancelado 😔.
                            </p>
                            
                            <div style='background-color: #f0f8ff; padding: 20px; border-left: 4px solid #3498db; margin: 20px 0;'>
                                <p style='margin: 0; font-size: 14px'>
                                    <strong>Razon:</strong> {reason}
                                </p>
                            </div>
                            
                            <p style='font-size: 16px; color: #333; line-height: 1.6;'>
                                Si tienes alguna duda o consulta sobre esta cancelación, no dudes en contactarnos. Estamos aquí para ayudarte.
                            </p>
                            <p><strong>Esperamos verte pronto de nuevo.</strong>
                            
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            
                            <p style='font-size: 14px; color: #777; text-align: center; margin: 0;'>
                                <strong>Censudex</strong> 
                            </p>
                        </div>
                    </div>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            await client.SendEmailAsync(msg);
        }
    }

}
