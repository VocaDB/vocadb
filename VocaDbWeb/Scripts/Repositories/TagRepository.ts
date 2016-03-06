
module vdb.repositories {

	import dc = vdb.dataContracts;

	export class TagRepository {
		
		constructor(private baseUrl: string) { }

		public create = (name: string, callback?: (result: dc.TagBaseContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags?name=" + name);
			$.post(url, callback);
		}

		public createReport = (tagId: number, reportType: string, notes: string, versionNumber: number, callback?: () => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/" + tagId + "/reports?" + helpers.AjaxHelper.createUrl({ reportType: [reportType], notes: [notes], versionNumber: [versionNumber] }));
			$.post(url, callback);

		}

		public getById = (id: number, fields: string, lang: string, callback?: (result: dc.TagApiContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/" + id);
			$.getJSON(url, { fields: fields || undefined, lang: lang }, callback);
		}

		public getComments = () => new EntryCommentRepository(new UrlMapper(this.baseUrl), "/tags/");

		public getList = (queryParams: TagQueryParams,
			callback?: (result: dc.PartialFindResultContract<dc.TagApiContract>) => void) => {

			var nameMatchMode = queryParams.nameMatchMode || models.NameMatchMode.Auto;

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags");
			var data = {
				start: queryParams.start, getTotalCount: queryParams.getTotalCount, maxResults: queryParams.maxResults,
				query: queryParams.query,
				fields: queryParams.fields || undefined,
				nameMatchMode: models.NameMatchMode[nameMatchMode],
				allowAliases: queryParams.allowAliases,
				categoryName: queryParams.categoryName,
				lang: queryParams.lang,
				sort: queryParams.sort
			};

			$.getJSON(url, data, callback);

		}

		public getTopTags = (lang: string, categoryName?: string, callback?: (tags: dc.TagBaseContract[]) => void) => {
			
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/tags/top");
			var data = { lang: lang, categoryName: categoryName };

			$.getJSON(url, data, callback);

		}

	}

	export interface TagQueryParams extends CommonQueryParams {
		
		allowAliases?: boolean;

		categoryName?: string;

		// Comma-separated list of optional fields
		fields?: string;

		sort?: string;

	}

}