using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace BooksGalore.Utility
{
	public class EmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var emailtosend = new MimeMessage();
			emailtosend.From.Add(MailboxAddress.Parse("irfansecondary@gmail.com"));
			emailtosend.To.Add(MailboxAddress.Parse(email));
			emailtosend.Subject= subject;
			emailtosend.Body=new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

			using(var emailclient=new SmtpClient())
			{
				emailclient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
				emailclient.Authenticate("irfansecondary@gmail.com", "vradakydmuunsgij");
				//this is done by usingtwo factor authentication and app passwords
				emailclient.Send(emailtosend);
				emailclient.Disconnect(true);
			}
			return Task.CompletedTask;
		}
	}
}
