
module vdb.repositories {

	export class VenueRepository extends BaseRepository {

		constructor(private readonly urlMapper: vdb.UrlMapper) {
			super(urlMapper.baseUrl);
		}

		public createReport = (venueId: number, reportType: string, notes: string, versionNumber: number, callback?: () => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/venues/" + venueId + "/reports?" + helpers.AjaxHelper.createUrl({ reportType: [reportType], notes: [notes], versionNumber: [versionNumber] }));
			$.post(url, callback);
		}

		public delete = (id: number, notes: string, hardDelete: boolean, callback?: () => void) => {
			$.ajax(this.urlMapper.mapRelative("/api/venues/" + id + "?hardDelete=" + hardDelete + "&notes=" + encodeURIComponent(notes)), { type: 'DELETE', success: callback });
		}
		
		public getList = (query: string, nameMatchMode: models.NameMatchMode, maxResults: number, callback?: (result: dc.PartialFindResultContract<dc.VenueForApiContract>) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/venues");
			var data = {
				query: query,
				maxResults: maxResults,
				nameMatchMode: models.NameMatchMode[nameMatchMode]
			};

			$.getJSON(url, data, callback);

		}

	}

}