import BaseRepository from './BaseRepository';
import CommentContract from '../DataContracts/CommentContract';
import EntryType from '../Models/EntryType';
import ICommentRepository from './ICommentRepository';
import UrlMapper from '../Shared/UrlMapper';

	export default class CommentRepository extends BaseRepository implements ICommentRepository {

		constructor(private urlMapper: UrlMapper, private entryType: EntryType) {
			super(urlMapper.baseUrl);
		}

		public createComment = (entryId: number, contract: CommentContract, callback: (contract: CommentContract) => void) => {

			contract.entry = { entryType: EntryType[this.entryType], id: entryId, name: null };
			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + EntryType[this.entryType] + "-comments"));
			$.postJSON(url, contract, callback, 'json');

		}

		public deleteComment = (commentId: number, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + EntryType[this.entryType] + "-comments/", commentId.toString()));
			$.ajax(url, { type: 'DELETE', success: callback });

		}

		public getComments = (listId: number, callback: (contract: CommentContract[]) => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + EntryType[this.entryType] + "-comments/"));
			$.getJSON(url, { entryId: listId }, result => callback(result.items));

		}

		public updateComment = (commentId: number, contract: CommentContract, callback?: () => void) => {

			var url = this.urlMapper.mapRelative(UrlMapper.buildUrl("api/comments/" + EntryType[this.entryType] + "-comments/", commentId.toString()));
			$.postJSON(url, contract, callback, 'json');

		}
	}