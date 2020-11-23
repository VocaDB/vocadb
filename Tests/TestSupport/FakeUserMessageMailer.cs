using System.Threading.Tasks;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.TestSupport
{
	public class FakeUserMessageMailer : IUserMessageMailer
	{
		public string Body { get; private set; }

		public string ToEmail { get; private set; }

		public string ReceiverName { get; private set; }

		public string Subject { get; private set; }

		public bool SendEmail(string toEmail, string receiverName, string subject, string body)
		{
			ToEmail = toEmail;
			ReceiverName = receiverName;
			Subject = subject;
			Body = body;

			return true;
		}

		public Task<bool> SendEmailAsync(string toEmail, string receiverName, string subject, string body)
		{
			return Task.FromResult(SendEmail(toEmail, receiverName, subject, body));
		}
	}
}
