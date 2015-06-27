
module vdb.repositories {

	import dc = vdb.dataContracts;

	// Repository for finding base class of common entry types.
	// Corresponds to the EntryApiController.
	export class EntryRepository {

		// Maps a relative URL to an absolute one.
		private mapUrl = (relative: string) => {
			return vdb.functions.mergeUrls(vdb.functions.mergeUrls(this.baseUrl, "/api/entries"), relative);
		};

		constructor(private baseUrl: string) {

		}

		getList = (paging: dc.PagingProperties, lang: string, query: string, tags: string[], fields: string, status: string, callback) => {

			var url = this.mapUrl("");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto',
				tag: tags,
				status: status
			};

			$.getJSON(url, data, callback);

		}

    }

}