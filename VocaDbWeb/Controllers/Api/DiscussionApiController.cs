using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/discussions")]	
	public class DiscussionApiController : ApiController {

		private readonly DiscussionQueries queries;
		private readonly IUserIconFactory userIconFactory;

		public DiscussionApiController(DiscussionQueries queries, IUserIconFactory userIconFactory) {
			this.queries = queries;
			this.userIconFactory = userIconFactory;
		}

		[Route("folders")]
		public IEnumerable<DiscussionFolderContract> GetFolders() {
			
			return queries.HandleQuery(ctx => {
				
				return ctx.Query()
					.Where(f => !f.Deleted)
					.OrderBy(f => f.SortIndex)
					.ThenBy(f => f.Name)
					.ToArray()
					.Select(f => new DiscussionFolderContract(f))
					.ToArray();

			});

		}

		[Route("folders/{folderId:int}/topics")]
		public IEnumerable<DiscussionTopicContract> GetTopics(int folderId) {
			
			return queries.HandleQuery(ctx => {
				
				var folder = ctx.Load(folderId);

				return folder.Topics
					.OrderByDescending(t => t.CreateDate)
					.Select(t => new DiscussionTopicContract(t, userIconFactory))
					.ToArray();

			});

		}
			
		[Route("folders")]
		public DiscussionFolderContract PostNewFolder(DiscussionFolderContract contract) {
			
			return queries.Create(contract);

		}

	}

}