import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PVType } from '@/Models/PVs/PVType';
import _ from 'lodash';

export class VideoServiceHelper {
	// TODO: Test.
	public static getPV = (
		allPVs: PVContract[],
		acceptFirst: boolean,
		predicates: ((pv: PVContract) => boolean)[],
	): PVContract | undefined => {
		if (allPVs.length === 0) return undefined;

		for (const predicate of predicates) {
			const pv = _.chain(allPVs).filter(predicate).first().value();

			if (pv) return pv;
		}

		return acceptFirst ? _.first(allPVs) : undefined;
	};

	// TODO: Test.
	public static primaryPV = (
		pvs: PVContract[],
		preferredService?: string,
	): PVContract | undefined => {
		const p = pvs.filter((pv) => !pv.disabled);

		if (preferredService) {
			return (
				VideoServiceHelper.getPV(
					p.filter((p) => p.service === preferredService),
					true,
					[
						(p): boolean => p.pvType === PVType[PVType.Original],
						(p): boolean => p.pvType === PVType[PVType.Reprint],
					],
				) ??
				VideoServiceHelper.getPV(p, true, [
					(p): boolean => p.pvType === PVType[PVType.Original],
					(p): boolean => p.pvType === PVType[PVType.Reprint],
				])
			);
		} else {
			return VideoServiceHelper.getPV(p, true, [
				(p): boolean => p.pvType === PVType[PVType.Original],
				(p): boolean => p.pvType === PVType[PVType.Reprint],
			]);
		}
	};
}
