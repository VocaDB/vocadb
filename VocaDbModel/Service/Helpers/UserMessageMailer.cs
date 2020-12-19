#nullable disable

using System;
using System.Net.Mail;
using System.Threading.Tasks;
using NLog;
using VocaDb.Model.Service.BrandableStrings;

namespace VocaDb.Model.Service.Helpers
{
	public class UserMessageMailer : IUserMessageMailer
	{
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly BrandableStringsManager _brandableStringsManager;

		public UserMessageMailer(BrandableStringsManager brandableStringsManager)
		{
			_brandableStringsManager = brandableStringsManager;
		}

		public bool SendEmail(string toEmail, string receiverName, string subject, string body)
		{
			if (string.IsNullOrEmpty(toEmail))
				return false;

			MailAddress to;

			try
			{
				to = new MailAddress(toEmail);
			}
			catch (FormatException x)
			{
				_log.Warn(x, "Unable to validate receiver email");
				return false;
			}

			var mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			mailMessage.Subject = subject;
			mailMessage.Body =
				string.Format(
					"Hi {0},\n\n" +
					"{1}" +
					"\n\n" +
					"- {2} mailer",
				receiverName, body, _brandableStringsManager.Layout.SiteName);

			var client = new SmtpClient();

			try
			{
				client.Send(mailMessage);
			}
			catch (SmtpException x)
			{
				_log.Error(x, "Unable to send mail");
				return false;
			}
			catch (InvalidOperationException x)
			{
				_log.Error(x, "Unable to send mail");
				return false;
			}

			return true;
		}

		public async Task<bool> SendEmailAsync(string toEmail, string receiverName, string subject, string body)
		{
			if (string.IsNullOrEmpty(toEmail))
				return false;

			MailAddress to;

			try
			{
				to = new MailAddress(toEmail);
			}
			catch (FormatException x)
			{
				_log.Warn(x, "Unable to validate receiver email");
				return false;
			}

			var mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			mailMessage.Subject = subject;
			mailMessage.Body =
				string.Format(
					"Hi {0},\n\n" +
					"{1}" +
					"\n\n" +
					"- {2} mailer",
				receiverName, body, _brandableStringsManager.Layout.SiteName);

			var client = new SmtpClient();

			try
			{
				await client.SendMailAsync(mailMessage);
			}
			catch (SmtpException x)
			{
				_log.Error(x, "Unable to send mail");
				return false;
			}
			catch (InvalidOperationException x)
			{
				_log.Error(x, "Unable to send mail");
				return false;
			}

			return true;
		}
	}
}
