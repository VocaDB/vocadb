import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';
import _ from 'lodash';

interface PiaproMetadata {
	Timestamp?: string;
}

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

	public static getPiaproTimestamp = (pv: PVContract): string | undefined => {
		const meta = pv.extendedMetadata
			? (JSON.parse(pv.extendedMetadata.json) as PiaproMetadata)
			: undefined;

		return meta?.Timestamp;
	};

	public static getPiaproUrlWithTimestamp = (
		pv: PVContract,
	): string | undefined => {
		const timestamp = VideoServiceHelper.getPiaproTimestamp(pv);

		if (timestamp === undefined) return undefined;

		return `https://cdn.piapro.jp/mp3_a/${pv.pvId.slice(0, 2)}/${
			pv.pvId
		}_${timestamp}_audition.mp3`;
	};

	public static readonly autoplayServices = [
		PVService.File,
		PVService.LocalFile,
		PVService.NicoNicoDouga,
		PVService.Vimeo,
		PVService.Youtube,
		PVService.SoundCloud,
	];

	public static canAutoplayPV = (pv: PVContract): boolean => {
		if (pv.service === PVService.Piapro)
			return VideoServiceHelper.getPiaproTimestamp(pv) !== undefined;

		return VideoServiceHelper.autoplayServices.includes(pv.service);
	};

	// TODO: Test.
	public static primaryPV = (
		pvs: PVContract[],
		preferredService?: PVService,
		autoplay?: boolean,
	): PVContract | undefined => {
		const p = autoplay
			? pvs.filter((pv) => !pv.disabled && VideoServiceHelper.canAutoplayPV(pv))
			: pvs.filter((pv) => !pv.disabled);

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
