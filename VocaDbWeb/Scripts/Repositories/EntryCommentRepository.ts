
module vdb.repositories {
	
	import dc = vdb.dataContracts;

	export class EntryCommentRepository implements ICommentRepository {

		private baseUrl: string;

		constructor(private urlMapper: UrlMapper, resourcePath: string) {
			this.baseUrl = UrlMapper.mergeUrls("/api/", resourcePath);
		}

		public createComment = (entryId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, entryId.toString(), "/comments"));
			$.post(url, contract, callback, 'json');

		}

		public deleteComment = (commentId: number, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, "/comments/", commentId.toString()));
			$.ajax(url, { type: 'DELETE', success: callback });

		}

		public getComments = (listId: number, callback: (contract: dc.CommentContract[]) => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, listId.toString(), "/comments/"));
			$.getJSON(url, result => callback(result.items));

		}

		public updateComment = (commentId: number, contract: dc.CommentContract, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, "/comments/", commentId.toString()));
			$.post(url, contract, callback, 'json');

		}

	}

}