using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public class DiscussionQueries : QueriesBase<IDiscussionFolderRepository, DiscussionFolder> {

		public DiscussionQueries(IDiscussionFolderRepository repository, IUserPermissionContext permissionContext) 
			: base(repository, permissionContext) {}

		public DiscussionFolderContract Create(DiscussionFolderContract contract) {
			
			// TODO
			PermissionContext.VerifyPermission(PermissionToken.Admin);

			return repository.HandleTransaction(ctx => {
				
				var folder = new DiscussionFolder {
					Name = contract.Name
				};

				ctx.Save(folder);

				return new DiscussionFolderContract(folder);

			});

		}

	}

}