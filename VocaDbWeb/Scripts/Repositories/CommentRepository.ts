
namespace vdb.repositories {

	import cls = vdb.models;

	export class CommentRepository extends BaseRepository implements ICommentRepository {

		constructor(private urlMapper: vdb.UrlMapper, private entryType: cls.EntryType) {
			super(urlMapper.baseUrl);
		}

		public createComment = (entryId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void) => {

			contract.entry = { entryType: cls.EntryType[this.entryType], id: entryId };
			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + cls.EntryType[this.entryType] + "-comments"));
			$.post(url, contract, callback, 'json');

		}

		public deleteComment = (commentId: number, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + cls.EntryType[this.entryType] + "-comments/", commentId.toString()));
			$.ajax(url, { type: 'DELETE', success: callback });

		}

		public getComments = (listId: number, callback: (contract: dc.CommentContract[]) => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + cls.EntryType[this.entryType] + "-comments/"));
			$.getJSON(url, { entryId: listId }, result => callback(result.items));

		}

		public updateComment = (commentId: number, contract: dc.CommentContract, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + cls.EntryType[this.entryType] + "-comments/", commentId.toString()));
			$.post(url, contract, callback, 'json');

		}
	}

}