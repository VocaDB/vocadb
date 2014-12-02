namespace VocaDb.Model.Service.Helpers {

	public interface IUserMessageMailer {

		bool SendEmail(string toEmail, string receiverName, string subject, string body);

	}

}