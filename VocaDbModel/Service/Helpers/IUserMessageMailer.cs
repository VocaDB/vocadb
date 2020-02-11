using System.Threading.Tasks;

namespace VocaDb.Model.Service.Helpers {

	public interface IUserMessageMailer {

		bool SendEmail(string toEmail, string receiverName, string subject, string body);
		Task<bool> SendEmailAsync(string toEmail, string receiverName, string subject, string body);

	}

}