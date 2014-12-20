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

		constructor(baseUrl: string, languagePreference = cls.globalization.ContentLanguagePreference.Default) {

			super(baseUrl, languagePreference);

            this.mapUrl = (relative) => {
                return vdb.functions.mergeUrls(baseUrl, "/Album") + relative;
            };

		}

		public findDuplicate = (params, callback: (result: dc.DuplicateEntryResultContract[]) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/Album/FindDuplicate");
			$.getJSON(url, params, callback);

		};

		public getForEdit = (id: number, callback: (result: dc.albums.AlbumForEditContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

		public getOne = (id: number, callback: (result: dc.AlbumContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums/" + id);
			$.getJSON(url, { fields: 'AdditionalNames', lang: this.languagePreferenceStr }, callback);
		}

		getList = (paging: dc.PagingProperties, lang: string, query: string, sort: string,
			discTypes: string, tag: string,
			artistId: number, artistParticipationStatus: string,
			childVoicebanks: boolean,
			fields: string,
			status: string,
			deleted: boolean,
			callback: (result: dc.PartialFindResultContract<dc.AlbumContract>) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/albums");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort,
				discTypes: discTypes,
				tag: tag,
				artistId: artistId,
				artistParticipationStatus: artistParticipationStatus,
				childVoicebanks: childVoicebanks,
				status: status,
				deleted: deleted
			};

			$.getJSON(url, data, callback);

		}

    }

	export interface AlbumQueryParams extends CommonQueryParams {

		discTypes: string;

	}

}