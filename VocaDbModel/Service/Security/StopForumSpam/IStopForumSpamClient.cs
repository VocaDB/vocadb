
using System.Threading.Tasks;

namespace VocaDb.Model.Service.Security.StopForumSpam {

	public interface IStopForumSpamClient {

		SFSResponseContract CallApi(string ip);
		Task<SFSResponseContract> CallApiAsync(string ip);

	}

}