/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

    // Repository for managing albums and related objects.
    // Corresponds to the AlbumController class.
    export class AlbumRepository extends BaseRepository {

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

		private urlMapper: UrlMapper;

		constructor(baseUrl: string, languagePreference = cls.globalization.ContentLanguagePreference.Default) {

			super(baseUrl, languagePreference);

			this.urlMapper = new UrlMapper(baseUrl);

            this.mapUrl = (relative) => {
                return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
            };

		}

		public createComment = (albumId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void) => {

			$.post(this.urlMapper.mapRelative("/api/albums/" + albumId + "/comments"), contract, callback, 'json');

		}

		public createReport = (albumId: number, reportType: string, notes: string, versionNumber: number, callback?: () => void) => {

			$.post(this.urlMapper.mapRelative("/Album/CreateReport"),
				{ reportType: reportType, notes: notes, albumId: albumId, versionNumber: versionNumber }, callback, 'json');

		}

		public deleteComment = (commentId: number, callback?: () => void) => {

			$.ajax(this.urlMapper.mapRelative("/api/albums/comments/" + commentId), { type: 'DELETE', success: callback });

		}

		public findDuplicate = (params, callback: (result: dc.DuplicateEntryResultContract[]) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/Album/FindDuplicate");
			$.getJSON(url, params, callback);

		};

		public getComments = (albumId: number, callback: (contract: dc.CommentContract[]) => void) => {

			$.getJSON(this.urlMapper.mapRelative("/api/albums/" + albumId + "/comments"), callback);

		}

		public getForEdit = (id: number, callback: (result: dc.albums.AlbumForEditContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

		public getOne = (id: number, callback: (result: dc.AlbumContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + id);
			$.getJSON(url, { fields: 'AdditionalNames', lang: this.languagePreferenceStr }, callback);
		}

		public getOneWithComponents = (id: number, fields: string, languagePreference: string, callback: (result: dc.AlbumForApiContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + id);
			$.getJSON(url, { fields: fields, lang: this.languagePreferenceStr }, callback);
		}

		getList = (paging: dc.PagingProperties, lang: string, query: string, sort: string,
			discTypes: string,
			tags: number[],
			childTags: boolean,
			artistIds: number[], artistParticipationStatus: string,
			childVoicebanks: boolean,
			includeMembers: boolean,
			fields: string,
			status: string,
			deleted: boolean,
			advancedFilters: viewModels.search.AdvancedSearchFilter[],
			callback: (result: dc.PartialFindResultContract<dc.AlbumContract>) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort,
				discTypes: discTypes,
				tagId: tags,
				childTags: childTags || undefined,
				artistId: artistIds,
				artistParticipationStatus: artistParticipationStatus,
				childVoicebanks: childVoicebanks,
				includeMembers: includeMembers || undefined,
				status: status,
				deleted: deleted,
				advancedFilters: advancedFilters
			};

			$.getJSON(url, data, callback);

		}

		public async getReviews(albumId: number) {

			const url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + albumId + "/reviews");
			const jqueryPromise = $.getJSON(url);

			const promise = Promise.resolve(jqueryPromise);
			return promise as Promise<dc.albums.AlbumReviewContract[]>;

		}

		public getTagSuggestions = (albumId: number, callback: (contract: dc.tags.TagUsageForApiContract[]) => void) => {
			$.getJSON(this.urlMapper.mapRelative("/api/albums/" + albumId + "/tagSuggestions"), callback);
		}

		public updateComment = (commentId: number, contract: dc.CommentContract, callback?: () => void) => {

			$.post(this.urlMapper.mapRelative("/api/albums/comments/" + commentId), contract, callback, 'json');

		}

		public updatePersonalDescription = (albumId: number, text: string, author: dc.ArtistContract) => {

			$.post(this.urlMapper.mapRelative("/api/albums/" + albumId + "/personal-description/"), { personalDescriptionText: text, personalDescriptionAuthor: author || undefined }, null, 'json');

		}

    }

	export interface AlbumQueryParams extends CommonQueryParams {

		discTypes: string;

	}

}