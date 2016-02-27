/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../DataContracts/DuplicateEntryResultContract.ts" />

module vdb.repositories {

	import cls = vdb.models;
    import dc = vdb.dataContracts;

    // Repository for managing artists and related objects.
    // Corresponds to the ArtistController class.
    export class ArtistRepository extends BaseRepository implements ICommentRepository {

		public createComment = (artistId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void) => {

			$.post(this.urlMapper.mapRelative("/api/artists/" + artistId + "/comments"), contract, callback, 'json');

		}

		public createReport = (artistId: number, reportType: string, notes: string, versionNumber: number, callback?: () => void) => {

			$.post(this.urlMapper.mapRelative("/Artist/CreateReport"),
				{ reportType: reportType, notes: notes, artistId: artistId, versionNumber: versionNumber }, callback, 'json');

		}

		public deleteComment = (commentId: number, callback?: () => void) => {

			$.ajax(this.urlMapper.mapRelative("/api/artists/comments/" + commentId), { type: 'DELETE', success: callback });

		}

        public findDuplicate: (params, callback: (result: dc.DuplicateEntryResultContract[]) => void ) => void;

		public getComments = (artistId: number, callback: (contract: dc.CommentContract[]) => void) => {

			$.getJSON(this.urlMapper.mapRelative("/api/artists/" + artistId + "/comments"), callback);

		}

		public getForEdit = (id: number, callback: (result: dc.artists.ArtistForEditContract) => void) => {
	
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists/" + id + "/for-edit");
			$.getJSON(url, callback);
					
		}

		public getOne = (id: number, callback: (result: dc.ArtistContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists/" + id);
			$.getJSON(url, { fields: 'AdditionalNames', lang: this.languagePreferenceStr }, callback);

		};

		public getList = (paging: dc.PagingProperties, lang: string, query: string, sort: string,
			artistTypes: string, tags: number[], followedByUserId: number, fields: string, status: string, callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort,
				artistTypes: artistTypes,
				tagId: tags,
				followedByUserId: followedByUserId,
				status: status
			};

			$.getJSON(url, data, callback);

		}

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

		private urlMapper: UrlMapper;

		public updateComment = (commentId: number, contract: dc.CommentContract, callback?: () => void) => {

			$.post(this.urlMapper.mapRelative("/api/artists/comments/" + commentId), contract, callback, 'json');

		}

        constructor(baseUrl: string, lang?: cls.globalization.ContentLanguagePreference) {

			super(baseUrl, lang);

			this.urlMapper = new UrlMapper(baseUrl);

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Artist") + relative;
            };

            this.findDuplicate = (params, callback: (result: dc.DuplicateEntryResultContract[]) => void ) => {

                $.post(this.mapUrl("/FindDuplicate"), params, callback);

            }

        }

	}

	export interface ArtistQueryParams extends CommonQueryParams {

		artistTypes: string;

	}

}