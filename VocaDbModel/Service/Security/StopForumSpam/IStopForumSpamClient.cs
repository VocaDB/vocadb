
using System.Threading.Tasks;

namespace VocaDb.Model.Service.Security.StopForumSpam
{

	public interface IStopForumSpamClient
	{

		Task<SFSResponseContract> CallApiAsync(string ip);

	}

}