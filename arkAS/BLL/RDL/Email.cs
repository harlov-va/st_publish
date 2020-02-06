using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;



namespace RDL
{
    public abstract class Email
    {
        public static string EmailFrom = "ruden-soft1@yandex.ru";
        public static string FromDisplayName = "Ruden CMS";
        public static string SmtpServer = "smtp.yandex.ru";
        public static string Username = "ruden-soft1@yandex.ru";
        public static string Password = "port10artur";
        public static int Port = 587; // 587;//465
        public static bool SSL = true;

        public static void SendMail(string to, string bcc, string cc, 
            string subject, string body)
        {
            SendMail(EmailFrom, FromDisplayName, to, bcc, cc, subject, 
                body, SmtpServer, Username, Password, Port);
        }        
        
        public static void SendMail(string from, string displayName, string to, string bcc, string cc, string subject, string body,
            string mailServer, string mailUsername, string mailPassword, int port, bool ssl = true, string attach = "")
        {
            if (String.IsNullOrEmpty(to)) return;


            // Instantiate a new instance of MailMessage
            MailMessage mMailMessage = new MailMessage();
            // Set the sender address of the mail message
            mMailMessage.From = new MailAddress(from, displayName);

            // Set the recepient address of the mail message
            if (attach != "")
            {
                mMailMessage.Attachments.Add(new Attachment(attach));
            }
            
            mMailMessage.To.Add(new MailAddress(to));

            // Check if the bcc value is null or an empty string
            if ((bcc != null) && (bcc != string.Empty))
            {
                // Set the Bcc address of the mail message
                mMailMessage.Bcc.Add(new MailAddress(bcc));
            }
            // Check if the cc value is null or an empty value
            if ((cc != null) && (cc != string.Empty))
            {
                // Set the CC address of the mail message
                mMailMessage.CC.Add(new MailAddress(cc));
            }       // Set the subject of the mail message
            mMailMessage.Subject = subject;
            // Set the body of the mail message
            mMailMessage.Body = body;

            // Set the format of the mail message body as HTML
            mMailMessage.IsBodyHtml = true;
            // Set the priority of the mail message to normal
            mMailMessage.Priority = MailPriority.Normal;

            // Instantiate a new instance of SmtpClient
            SmtpClient mSmtpClient = new SmtpClient(mailServer);
            mSmtpClient.Credentials = 
                new NetworkCredential(mailUsername, mailPassword);

            mSmtpClient.Port = port;
            mSmtpClient.EnableSsl = ssl;
            // Send the mail message
            mSmtpClient.Send(mMailMessage);
        }

        public static void SendMail(List<MailAddress> to, List<MailAddress> bcc, List<MailAddress> cc,
           string subject, string body)
        {  
         //   SendMail(CMSProvider.GetSetting("mail.from", "", 1), CMSProvider.GetSetting("mail.displayName", "", 1), to, bcc, cc, subject,
             //   body, CMSProvider.GetSetting("mail.server", "",1), CMSProvider.GetSetting("mail.username", "",1 ), CMSProvider.GetSetting("mail.password", "",1),
             //   RDL.Convert.StrToInt(CMSProvider.GetSetting("mail.port", "",1), 0));
               
        }     

        public static void SendMail(string from, string displayName, List<MailAddress> to, List<MailAddress> bcc, List<MailAddress> cc, string subject, string body,
            string mailServer, string mailUsername, string mailPassword, int port, bool ssl = true, string attach = "")
        {
            // Instantiate a new instance of MailMessage
            MailMessage mMailMessage = new MailMessage();
            // Set the sender address of the mail message
            mMailMessage.From = new MailAddress(from, displayName);
            // Set the recepient address of the mail message
            foreach(var m in to){
                mMailMessage.To.Add(m);    
            }
            if (attach != "") {
                mMailMessage.Attachments.Add(new Attachment(attach));
            }
            

            // Check if the bcc value is null or an empty string
            foreach (var m in bcc)
            {
                mMailMessage.Bcc.Add(m);
            }
            // Check if the cc value is null or an empty value
            foreach (var m in cc)
            {
                mMailMessage.CC.Add(m);
            }
            mMailMessage.Subject = subject;
            // Set the body of the mail message
            mMailMessage.Body = body;

            // Set the format of the mail message body as HTML
            mMailMessage.IsBodyHtml = true;
            // Set the priority of the mail message to normal
            mMailMessage.Priority = MailPriority.Normal;

            // Instantiate a new instance of SmtpClient
            SmtpClient mSmtpClient = new SmtpClient(mailServer);
            mSmtpClient.Credentials =
                new NetworkCredential(mailUsername, mailPassword);
            mSmtpClient.Port = port;
            mSmtpClient.EnableSsl = true;
            // Send the mail message
            mSmtpClient.Send(mMailMessage);
        }
    } 
}