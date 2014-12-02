
module vdb.repositories {

	import dc = vdb.dataContracts;

	export class PVRepository {
		
		constructor(private urlMapper: vdb.UrlMapper) { }

		public getPVByUrl = (pvUrl: string, type: string, success: (pv: dc.pvs.PVContract) => void) => {

			var url = this.urlMapper.mapRelative("/api/pvs");
			return $.getJSON(url, { pvUrl: pvUrl, type: type }, success);

		}

	}

}