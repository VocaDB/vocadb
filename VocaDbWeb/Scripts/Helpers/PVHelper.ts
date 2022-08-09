import { PVContract } from '@/DataContracts/PVs/PVContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { PVService } from '@/Models/PVs/PVService';

export class PVHelper {
	public static pvServicesArrayFromString = (
		pvServices: string,
	): PVService[] => {
		if (!pvServices) return [];

		var values = pvServices.split(',');
		var services: PVService[] = values.map(
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
