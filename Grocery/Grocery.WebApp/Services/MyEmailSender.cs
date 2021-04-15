using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;


namespace Grocery.WebApp.Services
{
    public class MyEmailSender : IEmailSender
    {

        private readonly IConfiguration _config;
        private readonly ILogger<MyEmailSender> _logger;

        public MyEmailSender(IConfiguration config, ILogger<MyEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }


        #region Microsoft.AspNetCore.Identity.UI.Services.IEmailSender members
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpServer = _config.GetValue<string>("MySmtpSettings:SmtpServer");
            var smtpServerSSL = _config.GetValue<bool>("MySmtpSettings:SmtpServerSSL");
            var smtpPort = _config.GetValue<int>("MySmtpSettings:SmtpPort");
            var smtpFromEmail = _config.GetValue<string>("MySmtpSettings:SmtpFromEmail");
            var smtpFromEmailAlias = _config.GetValue<string>("MySmtpSettings:SmtpFromEmailAlias");
            var smtpUsername = _config.GetValue<string>("MySmtpSettings:SmtpUsername");
            var smtpPassword = _config.GetValue<string>("MySmtpSettings:SmtpPassword");


            var client = new SmtpClient(smtpServer)
            {
                UseDefaultCredentials = false,
                EnableSsl = smtpServerSSL, //ssl is for enforcing security model, data transmitted using a secure port
                Port = smtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,

                // dont use default credentials, and provide my own credentials
                Credentials = new NetworkCredential(userName: smtpUsername, password: smtpPassword)
            }; 


            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpFromEmail, smtpFromEmailAlias),
                Subject = subject
            };


            // split multiple email addresses, add to collection, for each email in collection, call add() method
            mailMessage.To.Add(email);
            //mailMessage.To.Add(email2);
            //mailMessage.To.Add(email3);

            //mailMessage.Bcc.Add(email5);
            //mailMessage.Priority = MailPriority.High; // add exclamation in outlook to say that this is a high priority email
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlMessage;


            MyEmailSenderException myexception; //custom exception (created by us)

            try
            {
                client.SendMailAsync(mailMessage).Wait();
                return Task.CompletedTask;
            }
            catch (SmtpFailedRecipientsException exp)
            {
                myexception = new MyEmailSenderException($"Unable to send email to {exp.FailedRecipient}", exp);

            }
            catch (SmtpFailedRecipientException exp)
            {
                myexception = new MyEmailSenderException($"Unable to send email to {exp.FailedRecipient}", exp);

            }
            catch (SmtpException exp)
            {
                myexception = new MyEmailSenderException($"There was a problem sending email: {exp.Message}", exp);

            }
            catch (Exception exp)
            {
                myexception = new MyEmailSenderException($"Something went wrong! : {exp.Message}", exp);
            }


            return Task.FromException<MyEmailSenderException>(myexception);

        
            //throw new NotImplementedException();
        }
        #endregion


    }
}


/*
    "MySmtpSettings": {
    "SmtpServer": "smtp.mailtrap.io",
    "SmtpServerSSL": "true",
    "SmtpPort": "2525",Password
    "FromEmail": "support123@GroceryApp.com",
    "FromEmailAlias": "Grocery Support Team",
    "Username": "8a6bd0204af4b9",
    "": "1a5dc5e5d090ed"
  },
*/