
module vdb.repositories {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class ReleaseEventRepository extends BaseRepository {

		constructor(private urlMapper: vdb.UrlMapper) {
			super(urlMapper.baseUrl);
		}

		public getList = (queryParams: EventQueryParams,
			callback?: (result: dc.PartialFindResultContract<dc.ReleaseEventContract>) => void) => {

			var nameMatchMode = queryParams.nameMatchMode || models.NameMatchMode.Auto;

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/releaseEvents");
			var data = {
				start: queryParams.start, getTotalCount: queryParams.getTotalCount, maxResults: queryParams.maxResults,
				query: queryParams.query,
				category: queryParams.category || undefined,
				fields: queryParams.fields || undefined,
				userCollectionId: queryParams.userCollectionId || undefined,
				status: queryParams.status || undefined,
				nameMatchMode: models.NameMatchMode[nameMatchMode],
				lang: queryParams.lang,
				sort: queryParams.sort
			};

			$.getJSON(url, data, callback);

		}

		public getOne = (id: number, callback?: (result: dc.ReleaseEventContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/releaseEvents/" + id);
			$.getJSON(url, {}, result => callback(result && result.items && result.items.length ? result.items[0] : null));
		}

		public getOneByName = (name: string, callback?: (result: dc.ReleaseEventContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/releaseEvents?query=" + encodeURIComponent(name) + "&nameMatchMode=Exact&maxResults=1");
			$.getJSON(url, { }, result => callback(result && result.items && result.items.length ? result.items[0] : null));
		}

	}

	export interface EventQueryParams extends CommonQueryParams {

		category?: string;

		// Comma-separated list of optional fields
		fields?: string;

		sort?: string;

		status?: string;

		userCollectionId?: number;

	}

}