using VocaDb.Web.Code.Security;

namespace VocaDb.Tests.TestSupport {

	public class FakeStopForumSpamClient : IStopForumSpamClient {

		public FakeStopForumSpamClient() {
			Response = new SFSResponseContract();
		}

		public SFSResponseContract Response { get; set; }

		public SFSResponseContract CallApi(string ip) {
			if (Response != null)
				Response.IP = ip;
			return Response;
		}

	}

}
