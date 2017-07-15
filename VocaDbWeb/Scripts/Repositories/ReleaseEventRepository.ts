
module vdb.repositories {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class ReleaseEventRepository extends BaseRepository {

		constructor(private readonly urlMapper: vdb.UrlMapper) {
			super(urlMapper.baseUrl);
		}

		public createReport = (eventId: number, reportType: string, notes: string, versionNumber: number, callback?: () => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/releaseEvents/" + eventId + "/reports?" + helpers.AjaxHelper.createUrl({ reportType: [reportType], notes: [notes], versionNumber: [versionNumber] }));
			$.post(url, callback);
		}

		public delete = (id: number, notes: string, hardDelete: boolean, callback?: () => void) => {
			$.ajax(this.urlMapper.mapRelative("/api/releaseEvents/" + id + "?hardDelete=" + hardDelete + "&notes=" + encodeURIComponent(notes)), { type: 'DELETE', success: callback });
		}

		public deleteSeries = (id: number, notes: string, callback?: () => void) => {
			$.ajax(this.urlMapper.mapRelative("/api/releaseEventSeries/" + id + "?notes=" + encodeURIComponent(notes)), { type: 'DELETE', success: callback });
		}

		public getList = (queryParams: EventQueryParams,
			callback?: (result: dc.PartialFindResultContract<dc.ReleaseEventContract>) => void) => {

			var nameMatchMode = queryParams.nameMatchMode || models.NameMatchMode.Auto;

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/releaseEvents");
			var data = {
				start: queryParams.start, getTotalCount: queryParams.getTotalCount, maxResults: queryParams.maxResults,
				query: queryParams.query,
				category: queryParams.category || undefined,
				tagId: queryParams.tagIds,
				childTags: queryParams.childTags,
				fields: queryParams.fields || undefined,
				userCollectionId: queryParams.userCollectionId || undefined,
				artistId: queryParams.artistId || undefined,
				childVoicebanks: queryParams.childVoicebanks || undefined,
				includeMembers: queryParams.includeMembers || undefined,
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

		artistId?: number[];

		category?: string;

		childTags: boolean;

		childVoicebanks?: boolean;

		// Comma-separated list of optional fields
		fields?: string;

		includeMembers?: boolean;

		sort?: string;

		status?: string;

		tagIds: number[];

		userCollectionId?: number;

	}

}