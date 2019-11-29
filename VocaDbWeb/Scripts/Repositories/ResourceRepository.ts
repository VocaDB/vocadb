
//module vdb.repositories {

	import dc = vdb.dataContracts;

	export class ResourceRepository {
		
		constructor(private baseUrl: string) {}

		public getList = (cultureCode: string, setNames: string[], success: (resources: dc.ResourcesContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/resources/" + cultureCode + "/");
			$.getJSON(url, { setNames: setNames }, success);

		}

	}

//} 