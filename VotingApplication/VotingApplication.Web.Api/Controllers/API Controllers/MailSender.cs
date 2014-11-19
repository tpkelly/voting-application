﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace VotingApplication.Web.Api.Controllers.API_Controllers
{
    public class MailSender
    {
        public static void SendMail(string to, string subject, string message)
        {
            string hostEmail = WebConfigurationManager.AppSettings["HostEmail"];
            string hostPassword = WebConfigurationManager.AppSettings["HostPassword"];

            if (hostEmail == null || hostPassword == null)
            {
                return;
            }

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(hostEmail, hostPassword);

            MailMessage mail = new MailMessage(hostEmail, to);
            mail.Subject = subject;
            mail.Body = message;

            client.Send(mail);
        }
    }
}