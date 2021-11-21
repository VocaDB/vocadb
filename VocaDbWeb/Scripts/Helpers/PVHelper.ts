import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
import PVType from '@Models/PVs/PVType';
import _ from 'lodash';

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

	// TODO: Test.
	public static getPV = (
		allPVs: PVContract[],
		acceptFirst: boolean,
		predicates: ((pv: PVContract) => boolean)[],
	): PVContract | undefined => {
		if (allPVs.length === 0) return undefined;

		_.forEach(predicates, (predicate) => {
			const pv = _.chain(allPVs).filter(predicate).first().value();

			if (pv) return pv;
		});

		return acceptFirst ? _.first(allPVs) : undefined;
	};

	// TODO: Test.
	public static primaryPV = (pvs: PVContract[]): PVContract | undefined => {
		const preferredVideoService = vdb.values.loggedUser?.preferredVideoService;

		const p = pvs.filter((pv) => !pv.disabled);

		if (preferredVideoService) {
			return (
				PVHelper.getPV(
					p.filter((p) => p.service === preferredVideoService),
					true,
					[
						(p): boolean => p.pvType === PVType[PVType.Original],
						(p): boolean => p.pvType === PVType[PVType.Reprint],
					],
				) ??
				PVHelper.getPV(p, true, [
					(p): boolean => p.pvType === PVType[PVType.Original],
					(p): boolean => p.pvType === PVType[PVType.Reprint],
				])
			);
		} else {
			return PVHelper.getPV(p, true, [
				(p): boolean => p.pvType === PVType[PVType.Original],
				(p): boolean => p.pvType === PVType[PVType.Reprint],
			]);
		}
	};
}
