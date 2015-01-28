using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts;
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
			
		[Route("topics/{topicId:int}")]
		[Authorize]
		public void PostEditTopic(int topicId, DiscussionTopicContract contract) {
			
			queries.UpdateTopic(topicId, contract);

		}

		[Route("topics/{topicId:int}/comments")]
		[Authorize]
		public CommentContract PostNewComment(int topicId, CommentContract contract) {
			
			return queries.CreateComment(topicId, contract);

		}

		[Route("folders")]
		[Authorize]
		public DiscussionFolderContract PostNewFolder(DiscussionFolderContract contract) {
			
			return queries.CreateFolder(contract);

		}

		[Route("folders/{folderId:int}/topics")]
		[Authorize]
		public DiscussionTopicContract PostNewTopic(int folderId, DiscussionTopicContract contract) {
			
			return queries.CreateTopic(folderId, contract);

		}

	}

}