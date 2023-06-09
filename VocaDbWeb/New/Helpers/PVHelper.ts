import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { UserWithPermissionsContract } from '@/types/DataContracts/User/UserWithPermissionsContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { PVService } from '@/types/Models/PVs/PVService';

export class PVHelper {
	static pvServicesArrayFromString = (pvServices: string): PVService[] => {
		if (!pvServices) return [];

		var values = pvServices.split(',');
		var services: PVService[] = values.map((val) => val.trim() as PVService);

		return services;
	};

	static primaryPV = (
		pvs: PVContract[],
		loggedUser: UserWithPermissionsContract | undefined,
		autoplay?: boolean
	): PVContract | undefined => {
		return VideoServiceHelper.primaryPV(pvs, loggedUser?.preferredVideoService, autoplay);
	};
}
