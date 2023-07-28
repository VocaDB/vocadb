import { PVContract } from '@/types/DataContracts/PVs/PVContract';

const PREFERRED_SERVICES = ['Youtube', 'NicoNicoDouga', 'Bilibili', 'Vimeo'];

export const getBestThumbUrl = (pvs: PVContract[]): string | undefined => {
	return pvs
		.filter((pv) => !pv.disabled && pv.url !== undefined)
		.reduce((currPV: PVContract | undefined, nextPV) => {
			const currPos = PREFERRED_SERVICES.indexOf(currPV?.service ?? '');
			const nextPos = PREFERRED_SERVICES.indexOf(nextPV.service ?? '');
			if (
				currPV === undefined ||
				(PREFERRED_SERVICES.includes(nextPV.service) && nextPos < currPos)
			) {
				return nextPV;
			}
			return currPV;
		}, undefined)?.url;
};
