using System;
using System.Net.Mail;
using NLog;
using VocaDb.Model.Service.BrandableStrings;

namespace VocaDb.Model.Service.Helpers {

	public class UserMessageMailer : IUserMessageMailer {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly BrandableStringsManager brandableStringsManager;

		public UserMessageMailer(BrandableStringsManager brandableStringsManager) {
			this.brandableStringsManager = brandableStringsManager;
		}

		public bool SendEmail(string toEmail, string receiverName, string subject, string body) {

			if (string.IsNullOrEmpty(toEmail))
				return false;

			MailAddress to;

			try {
				to = new MailAddress(toEmail);
			} catch (FormatException x) {
				log.Warn("Unable to validate receiver email", x);
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
				receiverName, body, brandableStringsManager.Layout.SiteName);

			var client = new SmtpClient();

			try {
				client.Send(mailMessage);
			} catch (SmtpException x) {
				log.Error("Unable to send mail", x);
				return false;
			} catch (InvalidOperationException x) {
				log.Error("Unable to send mail", x);
				return false;				
			}

			return true;

		}

	}

}
