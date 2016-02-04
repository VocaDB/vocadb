
module vdb.repositories {

	import dc = vdb.dataContracts;

	export class TagRepository {
		
		constructor(private baseUrl: string) { }

		public getById = (id: number, fields: string, callback?: (result: dc.TagApiContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/" + id);
			$.getJSON(url, { fields: fields || undefined }, callback);
		}

		public getComments = () => new EntryCommentRepository(new UrlMapper(this.baseUrl), "/tags/");

		public getList = (paging: dc.PagingProperties, lang: string, query: string,
			sort: string,
			allowAliases: boolean, categoryName: string, callback?: (result: dc.PartialFindResultContract<dc.TagApiContract>) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: "AdditionalNames,MainPicture", nameMatchMode: 'Auto',
				allowAliases: allowAliases,
				categoryName: categoryName,
				lang: lang,
				sort: sort
			};

			$.getJSON(url, data, callback);

		}

		public getTopTags = (lang: string, categoryName?: string, callback?: (tags: dc.TagBaseContract[]) => void) => {
			
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/top");
			var data = { lang: lang, categoryName: categoryName };

			$.getJSON(url, data, callback);

		}

	}

}