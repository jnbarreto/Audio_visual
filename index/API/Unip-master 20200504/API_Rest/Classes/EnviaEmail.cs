using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using API_Rest.Models;
using System.Data;

namespace API_Rest.Classes
{
    public class EnviaEmail
    {
        public bool sendEmail(string body = "", List<Email> dadosEmail = null, string assunto = "")
        {
            DB Conn = new DB();

            bool envioEmail = true;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("mja.cursos@gmail.com", "cursos@mja");
            
            MailMessage mail = new MailMessage();
            mail.Sender = new System.Net.Mail.MailAddress("mja.cursos@gmail.com", "Unip Audio Visual");
            mail.From = new MailAddress("mja.cursos@gmail.com", "Audio Visual");

            
            for (int i = 0; i < dadosEmail.Count; i++)
            {
                mail.To.Add(new MailAddress(dadosEmail[i].email, dadosEmail[i].nome));
            }

            mail.Subject = assunto;

            mail.Body = "Aviso Unip Audio Visual:<br/>" +
                        "<hr>" +
                        body;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);
                Conn.debug("E-mail enviado com sucesso. ", "", true);
            }
            catch (Exception e)
            {
                Conn.debug("Falha no envio de E-mail. " + e.Message, "", true);
            }
            finally
            {
                mail = null;
                dadosEmail = null;
            }

            return envioEmail;
        }
    }
}