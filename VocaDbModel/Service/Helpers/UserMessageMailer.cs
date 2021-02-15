#nullable disable

using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NLog;
using VocaDb.Model.Service.BrandableStrings;

namespace VocaDb.Model.Service.Helpers
{
	public record SmtpSettings
	{
		public const string Smtp = "Smtp";

		public string From { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}

	public class UserMessageMailer : IUserMessageMailer
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly BrandableStringsManager _brandableStringsManager;
		private readonly SmtpSettings _smtpSettings;

		public UserMessageMailer(BrandableStringsManager brandableStringsManager, IOptions<SmtpSettings> smtpSettings)
		{
			_brandableStringsManager = brandableStringsManager;
			_smtpSettings = smtpSettings.Value;
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
				s_log.Warn(x, "Unable to validate receiver email");
				return false;
			}

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_smtpSettings.From),
				Subject = subject,
				Body =
					$"Hi {receiverName},\n\n" +
					$"{body}" +
					$"\n\n" +
					$"- {_brandableStringsManager.Layout.SiteName} mailer",
			};
			mailMessage.To.Add(to);

			var client = new SmtpClient(_smtpSettings.Host)
			{
				Port = _smtpSettings.Port,
				Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
				EnableSsl = true,
			};

			try
			{
				client.Send(mailMessage);
			}
			catch (SmtpException x)
			{
				s_log.Error(x, "Unable to send mail");
				return false;
			}
			catch (InvalidOperationException x)
			{
				s_log.Error(x, "Unable to send mail");
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
				s_log.Warn(x, "Unable to validate receiver email");
				return false;
			}

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_smtpSettings.From),
				Subject = subject,
				Body =
					$"Hi {receiverName},\n\n" +
					$"{body}" +
					$"\n\n" +
					$"- {_brandableStringsManager.Layout.SiteName} mailer",
			};
			mailMessage.To.Add(to);

			var client = new SmtpClient(_smtpSettings.Host)
			{
				Port = _smtpSettings.Port,
				Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
				EnableSsl = true,
			};

			try
			{
				await client.SendMailAsync(mailMessage);
			}
			catch (SmtpException x)
			{
				s_log.Error(x, "Unable to send mail");
				return false;
			}
			catch (InvalidOperationException x)
			{
				s_log.Error(x, "Unable to send mail");
				return false;
			}

			return true;
		}
	}
}
