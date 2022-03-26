import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
import _ from 'lodash';

import VideoServiceHelper from './VideoServiceHelper';

export default class PVHelper {
	public static pvServicesArrayFromString = (
		pvServices: string,
	): PVService[] => {
		if (!pvServices) return [];

		var values = pvServices.split(',');
		var services: PVService[] = _.map(
			values,
			(val) => PVService[val.trim() as keyof typeof PVService],
		);

		return services;
	};

	public static primaryPV = (pvs: PVContract[]): PVContract | undefined => {
		return VideoServiceHelper.primaryPV(
			pvs,
			vdb.values.loggedUser?.preferredVideoService,
		);
	};
}
