using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ARSFD.Web.Services
{
	public class EmailSender : IEmailSender
	{
		public bool Enable { get; set; } = false;

		public async Task SendEmailAsync(string email, string subject, string message)
		{
			if (Enable == false)
			{
				return;
			}

			try
			{
				using (var client = new SmtpClient("smtp.gmail.com", 587))
				{
					client.EnableSsl = true;
					client.DeliveryMethod = SmtpDeliveryMethod.Network;
					client.UseDefaultCredentials = false;
					client.Credentials = new NetworkCredential("projectarsfd@gmail.com", "projectarsfd12");
					client.Timeout = TimeSpan.FromSeconds(2).Milliseconds;

					using (var mailMessage = new MailMessage())
					{
						mailMessage.From = new MailAddress("projectarsfd@gmail.com");
						mailMessage.To.Add(email);
						mailMessage.Body = message;
						mailMessage.Subject = subject;

						await client.SendMailAsync(mailMessage);
					}
				}
			}
			catch
			{

			}
		}
	}
}
