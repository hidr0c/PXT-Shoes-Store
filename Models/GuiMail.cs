using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace VinaShoseShop.Models
{
	public class GuiMail
	{
        private static string Email = "seafoodtavern@gmail.com";
        private static string password = "cpit xeeo rsai xmcs";
        public static bool SendMail(string name, string subject, string content,
		 string toMail)
		{
			bool rs = false;
			try
			{
				MailMessage message = new MailMessage();
				var smtp = new SmtpClient();
				{
					smtp.Host = "smtp.gmail.com"; //host name
					smtp.Port = 587; //port number
					smtp.EnableSsl = true; //whether your smtp server requires SSL
					smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

					smtp.UseDefaultCredentials = false;
					smtp.Credentials = new NetworkCredential()
					{
						UserName = Email,
						Password = password
					};
				}
				MailAddress fromAddress = new MailAddress(Email, name);
				message.From = fromAddress;
				message.To.Add(toMail);
				message.Subject = subject;
				message.IsBodyHtml = true;
				message.Body = content;
				smtp.Send(message);
				rs = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				rs = false;
			}
			return rs;
		}
	}
}