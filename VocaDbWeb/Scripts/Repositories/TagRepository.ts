
module vdb.repositories {

	import dc = vdb.dataContracts;

	export class TagRepository {
		
		constructor(private baseUrl: string) { }

		public create = (name: string, callback?: (result: dc.TagBaseContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags?name=" + name);
			$.post(url, callback);
		}

		public getById = (id: number, fields: string, lang: string, callback?: (result: dc.TagApiContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/" + id);
			$.getJSON(url, { fields: fields || undefined, lang: lang }, callback);
		}

		public getComments = () => new EntryCommentRepository(new UrlMapper(this.baseUrl), "/tags/");

		public getList = (paging: dc.PagingProperties, lang: string, query: string,
			nameMatchMode: models.NameMatchMode,
			sort: string,
			allowAliases: boolean, categoryName: string,
			fields: string,
			callback?: (result: dc.PartialFindResultContract<dc.TagApiContract>) => void) => {

			nameMatchMode = nameMatchMode || models.NameMatchMode.Auto;

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query,
				fields: fields || undefined,
				nameMatchMode: models.NameMatchMode[nameMatchMode],
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