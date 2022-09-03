import { PVContract } from '@/DataContracts/PVs/PVContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { PVService } from '@/Models/PVs/PVService';

interface PiaproMetadata {
	Timestamp?: string;
}

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

	public static getPiaproTimestamp = (pv: PVContract): string | undefined => {
		const meta = pv.extendedMetadata
			? (JSON.parse(pv.extendedMetadata.json) as PiaproMetadata)
			: undefined;

		return meta?.Timestamp;
	};

	public static getPiaproUrlWithTimestamp = (
		pv: PVContract,
	): string | undefined => {
		const timestamp = PVHelper.getPiaproTimestamp(pv);

		if (timestamp === undefined) return undefined;

		return `https://cdn.piapro.jp/mp3_a/${pv.pvId.slice(0, 2)}/${
			pv.pvId
		}_${timestamp}_audition.mp3`;
	};

	private static readonly autoplayServices = [
		PVService.File,
		PVService.LocalFile,
		PVService.NicoNicoDouga,
		PVService.Vimeo,
		PVService.Youtube,
		PVService.SoundCloud,
	];

	public static canAutoplayPV = (pv: PVContract): boolean => {
		if (pv.service === PVService[PVService.Piapro])
			return PVHelper.getPiaproTimestamp(pv) !== undefined;

		return PVHelper.autoplayServices.includes(
			PVService[pv.service as keyof typeof PVService],
		);
	};
}
