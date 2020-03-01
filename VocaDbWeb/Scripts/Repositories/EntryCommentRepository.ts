import CommentContract from '../DataContracts/CommentContract';
import ICommentRepository from './ICommentRepository';
import UrlMapper from '../Shared/UrlMapper';

//module vdb.repositories {
	
	export default class EntryCommentRepository implements ICommentRepository {

		private baseUrl: string;

		constructor(private urlMapper: UrlMapper, resourcePath: string) {
			this.baseUrl = UrlMapper.mergeUrls("/api/", resourcePath);
		}

		public createComment = (entryId: number, contract: CommentContract, callback: (contract: CommentContract) => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, entryId.toString(), "/comments"));
			$.post(url, contract, callback, 'json');

		}

		public deleteComment = (commentId: number, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, "/comments/", commentId.toString()));
			$.ajax(url, { type: 'DELETE', success: callback });

		}

		public getComments = (listId: number, callback: (contract: CommentContract[]) => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, listId.toString(), "/comments/"));
			$.getJSON(url, result => callback(result.items));

		}

		public updateComment = (commentId: number, contract: CommentContract, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl(this.baseUrl, "/comments/", commentId.toString()));
			$.post(url, contract, callback, 'json');

		}

	}

//}