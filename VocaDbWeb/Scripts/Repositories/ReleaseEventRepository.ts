
module vdb.repositories {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class ReleaseEventRepository extends BaseRepository {

		constructor(private urlMapper: vdb.UrlMapper) {
			super(urlMapper.baseUrl);
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

}