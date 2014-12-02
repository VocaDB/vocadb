
module vdb.repositories {

	import dc = vdb.dataContracts;

	export class TagRepository {
		
		constructor(private baseUrl: string) { }

		public getList = (paging: dc.PagingProperties, query: string,
			allowAliases: boolean, categoryName: string, callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: "MainPicture", nameMatchMode: 'Auto',
				allowAliases: allowAliases,
				categoryName: categoryName
			};

			$.getJSON(url, data, callback);

		}

		public getTopTags = (categoryName?: string, callback?) => {
			
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/top");
			var data = { categoryName: categoryName };

			$.getJSON(url, data, callback);

		}

	}

}