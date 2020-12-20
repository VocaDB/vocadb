import PVService from '../Models/PVs/PVService';

	export default class PVHelper {
		
		public static pvServicesArrayFromString = (pvServices: string) => {

			if (!pvServices)
				return [];

			var values = pvServices.split(",");
			var services: PVService[] = _.map(values, val => PVService[val.trim()]);

			return services;

		}

	}