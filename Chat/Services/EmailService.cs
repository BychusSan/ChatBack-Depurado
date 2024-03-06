using System.Net.Mail;
using System.Net;
using Chat.Models;
using Chat.DTOs;
using System;
using static System.Net.WebRequestMethods;

namespace Chat.Services
{
    public class EmailService
    {
        public async Task EnviarCorreo(string emailDestino, Usuario usuario)
        {
            string user = "a7d7524deda88f";
            string contraseña = "fa0afebb0f24b7";
            string servidorSmtp = "sandbox.smtp.mailtrap.io";
            int puertoSmtp = 2525;

            var clienteSmtp = new SmtpClient(servidorSmtp, puertoSmtp)
            {
                Credentials = new NetworkCredential(user, contraseña),
                EnableSsl = true
            };

            var asunto = "¡Bienvenido a ConnecTalk!";
            var cuerpo =
                @"<h2>Bienvenido a ConnecTalk!</h2>" +
                 "<p>Gracias por registrarte en nuestra plataforma.</p>" +
                 "<p>Estos son tus datos de usuario:</p>" +
                 "<ul>" +
                 "<li><strong>Nombre de usuario:</strong> " + usuario.Nombre + "</li>" +
                 "<li><strong>Email:</strong> " + usuario.Email + "</li>" +
                 "<li><strong>Rol:</strong> " + usuario.Rol + "</li>" +
                 "<li><strong>Sala:</strong> " + usuario.Room + "</li>" +
                 "</ul>" +
                 "<p>Por favor, no dudes en ponerte en contacto con nosotros si necesitas ayuda o tienes alguna pregunta!</p>" +

                 "<h6>:)<h6>";
            var mensaje = new MailMessage("connectalk.soporte@gmail.com", emailDestino, asunto, cuerpo);
            mensaje.IsBodyHtml = true;
            try
            {
                clienteSmtp.Send(mensaje);
                Console.WriteLine("Correo electrónico enviado.");
            }
            catch (Exception ex)
            {

                throw new Exception($"Error al enviar el email !!: {ex.Message}");
            }
        }



        public async Task EnviarCorreoElectronico(string destinatario, string asunto, string cuerpo)
        {
            var fromAddress = new MailAddress("connectalk.soporte@gmail.com", "ConnecTalk");
            var toAddress = new MailAddress(destinatario);
            const string fromPassword = "bwuoielkercejhln"; //Contraseña de aplicación, NO es la real de acceso = (proyectogrupo4)
            string subject = asunto;
            string body = cuerpo;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com", // Servidor SMTP de Gmail
                Port = 587, // Puerto SMTP de Gmail
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }

        public async Task EnviarCorreoElectronico(string destinatario, Usuario usuario, string contrasena)
        {
            var fromAddress = new MailAddress("connectalk.soporte@gmail.com", "ConnecTalk");
            var toAddress = new MailAddress(destinatario);
            const string fromPassword = "bwuoielkercejhln"; // Contraseña de aplicación en configuración del gmail, no es la de acceso al gmail
            string asunto = "¡Bienvenido a ConnecTalk!";
                //string URLlogo = "https://media.discordapp.net/attachments/1159796578236780554/1211988587550343238/ConnecTalk-logo-final.png?ex=65f03354&is=65ddbe54&hm=711f4a3e7bd59c4eda26d61335fbb97105e4dfdbdae37da430de879c2143b80a&=&format=webp&quality=lossless&width=1000&height=1000";
            string cuerpo =
                 @" <h2> Bienvenido a ConnecTalk!</h2> " +
                 "<p>Gracias por registrarte en nuestra plataforma.</p>" +
                 "<p>Estos son tus datos de usuario:</p>" +
                 "<ul>" +
                 "<li><strong>Nombre de usuario:</strong> " + usuario.Nombre + "</li>" +
                 "<li><strong>Email:</strong> " + usuario.Email + "</li>" +
                 "<li><strong>Rol:</strong> " + usuario.Rol + "</li>" +
                 "<li><strong>Sala:</strong> " + usuario.Room + "</li>" +
                 "<li><strong>\nPassword:</strong> " + contrasena + "</li>" +

                 "</ul>" +
                 "<p>Por favor, no dudes en ponerte en contacto con nosotros si necesitas ayuda o tienes alguna pregunta!</p>" +

                 "<p>URL acceso: <a href=\"http://gabrielsan-001-site1.ftempurl.com/dist\">ConnecTalk</a></p>\r\n" +


                 "<h6>:)<h6>" +

            "<img src='https://media.discordapp.net/attachments/1159796578236780554/1211988587550343238/ConnecTalk-logo-final.png?ex=65f03354&is=65ddbe54&hm=711f4a3e7bd59c4eda26d61335fbb97105e4dfdbdae37da430de879c2143b80a&=&format=webp&quality=lossless&width=1000&height=1000'" +
            " alt='ConnecTalk Logo' style='border-radius:50%;background: none;width: 200px; height: 200px;'>";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);

            }
        }
    }
}