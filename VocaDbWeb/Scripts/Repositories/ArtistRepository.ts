/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../DataContracts/ArtistContract.ts" />
/// <reference path="../DataContracts/DuplicateEntryResultContract.ts" />

module vdb.repositories {

	import cls = vdb.models;
    import dc = vdb.dataContracts;

    // Repository for managing artists and related objects.
    // Corresponds to the ArtistController class.
    export class ArtistRepository extends BaseRepository {

        public findDuplicate: (params, callback: (result: dc.DuplicateEntryResultContract[]) => void ) => void;

		public getForEdit = (id: number, callback: (result: dc.artists.ArtistForEditContract) => void) => {
	
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists/" + id + "/for-edit");
			$.getJSON(url, callback);
					
		}

		public getOne = (id: number, callback: (result: dc.ArtistContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists/" + id);
			$.getJSON(url, { fields: 'AdditionalNames', lang: this.languagePreferenceStr }, callback);

		};

		public getList = (paging: dc.PagingProperties, lang: string, query: string, sort: string,
			artistTypes: string, tag: string, followedByUserId: number, fields: string, status: string, callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/artists");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort,
				artistTypes: artistTypes,
				tag: tag,
				followedByUserId: followedByUserId,
				status: status
			};

			$.getJSON(url, data, callback);

		}

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(baseUrl: string, lang?: cls.globalization.ContentLanguagePreference) {

			super(baseUrl, lang);

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Artist") + relative;
            };

            this.findDuplicate = (params, callback: (result: dc.DuplicateEntryResultContract[]) => void ) => {

                $.post(this.mapUrl("/FindDuplicate"), params, callback);

            }

        }

    }

}