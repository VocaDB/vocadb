
//module vdb.helpers {

	import cls = vdb.models;

	export class PVHelper {
		
		public static pvServicesArrayFromString = (pvServices: string) => {

			if (!pvServices)
				return [];

			var values = pvServices.split(",");
			var services: cls.pvs.PVService[] = _.map(values, val => cls.pvs.PVService[val.trim()]);

			return services;

		}

	}

//}