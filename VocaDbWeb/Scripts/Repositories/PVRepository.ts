
import PVContract from '../DataContracts/PVs/PVContract';
import UrlMapper from '../Shared/UrlMapper';

//module vdb.repositories {

	export default class PVRepository {
		
		constructor(private readonly  urlMapper: UrlMapper) { }

		public getPVByUrl = (pvUrl: string, type: string, success: (pv: PVContract) => void) => {

			var url = this.urlMapper.mapRelative("/api/pvs");
			return $.getJSON(url, { pvUrl: pvUrl, type: type }, success);

		}

	}

//}