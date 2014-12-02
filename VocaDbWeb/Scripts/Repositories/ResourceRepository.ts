
module vdb.repositories {
	
	export class ResourceRepository {
		
		constructor(private baseUrl: string) {}

		public getList = (cultureCode: string, setNames: string[], success) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/resources/" + cultureCode + "/");
			$.getJSON(url, { setNames: setNames }, success);

		}

	}

} 