
namespace VocaDb.Model.Service.Security.StopForumSpam {

	public interface IStopForumSpamClient {

		SFSResponseContract CallApi(string ip);

	}

}