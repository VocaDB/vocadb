using System.Threading.Tasks;
using VocaDb.Model.Service.Security.StopForumSpam;

namespace VocaDb.Tests.TestSupport
{
	public class FakeStopForumSpamClient : IStopForumSpamClient
	{
		public FakeStopForumSpamClient()
		{
			Response = new SFSResponseContract();
		}

		public SFSResponseContract Response { get; set; }

		private SFSResponseContract CallApi(string ip)
		{
			if (Response != null)
				Response.IP = ip;
			return Response;
		}

		public Task<SFSResponseContract> CallApiAsync(string ip) => Task.FromResult(CallApi(ip));
	}
}
