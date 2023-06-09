import {
	BandcampPVContract,
	PiaproPVContract,
	PVContract,
	SoundCloudPVContract,
} from '@/types/DataContracts/PVs/PVContract';
import { PVService } from '@/types/Models/PVs/PVService';
import { PVType } from '@/types/Models/PVs/PVType';

interface BandcampMetadata {
	Url?: string;
}

interface PiaproMetadata {
	Timestamp?: string;
}

export class VideoServiceHelper {
	// TODO: Test.
	static getPV = (
		allPVs: PVContract[],
		acceptFirst: boolean,
		predicates: ((pv: PVContract) => boolean)[]
	): PVContract | undefined => {
		if (allPVs.length === 0) return undefined;

		for (const predicate of predicates) {
			const [pv] = allPVs.filter(predicate);

			if (pv) return pv;
		}

		return acceptFirst ? allPVs[0] : undefined;
	};

	static getPiaproTimestamp = (pv: PiaproPVContract): string | undefined => {
		const meta =
			pv.extendedMetadata && pv.extendedMetadata.json
				? (JSON.parse(pv.extendedMetadata.json) as PiaproMetadata)
				: undefined;

		return meta?.Timestamp;
	};

	private static getPiaproUrlWithTimestamp = (pv: PiaproPVContract): string | undefined => {
		const timestamp = VideoServiceHelper.getPiaproTimestamp(pv);

		if (timestamp === undefined) return undefined;

		return `https://cdn.piapro.jp/mp3_a/${pv.pvId.slice(0, 2)}/${
			pv.pvId
		}_${timestamp}_audition.mp3`;
	};

	private static getSoundCloudUrlById = (pv: SoundCloudPVContract): string => {
		const parts = pv.pvId.split(' ');
		const url = `https://api.soundcloud.com/tracks/${parts[0]}`;
		return url;
	};

	static getVideoId = (pv: PVContract): string | undefined => {
		switch (pv.service) {
			case PVService.Piapro:
				return VideoServiceHelper.getPiaproUrlWithTimestamp(pv);

			case PVService.SoundCloud:
				return VideoServiceHelper.getSoundCloudUrlById(pv);

			default:
				return pv.pvId;
		}
	};

	private static getBandcampUrlById = (pv: BandcampPVContract): string => {
		const bandcampMetadata =
			pv.extendedMetadata && pv.extendedMetadata.json
				? (JSON.parse(pv.extendedMetadata.json) as BandcampMetadata)
				: undefined;
		return bandcampMetadata?.Url ?? `https://bandcamp.com/track/${pv.pvId}`;
	};

	static getUrlById = (pv: PVContract): string => {
		switch (pv.service) {
			case PVService.Bandcamp:
				return VideoServiceHelper.getBandcampUrlById(pv);

			case PVService.Bilibili:
				return `https://www.bilibili.com/video/av${pv.pvId}`;

			case PVService.Creofuga:
				return `https://creofuga.net/audios/${pv.pvId}`;

			case PVService.File:
				return pv.pvId;

			case PVService.LocalFile:
				return 'unsupported';

			case PVService.NicoNicoDouga:
				return `https://www.nicovideo.jp/watch/${pv.pvId}`;

			case PVService.Piapro:
				return `https://piapro.jp/content/${pv.pvId}`;

			case PVService.SoundCloud:
				return `https://soundcloud.com/${pv.pvId.split(' ')[1]}`;

			case PVService.Vimeo:
				return `https://vimeo.com/${pv.pvId}`;

			case PVService.Youtube:
				return `https://www.youtube.com/watch?v=${pv.pvId}`;
		}
	};

	static readonly autoplayServices = [
		PVService.File,
		PVService.LocalFile,
		PVService.NicoNicoDouga,
		PVService.Vimeo,
		PVService.Youtube,
		PVService.SoundCloud,
	];

	static canAutoplayPV = (pv: PVContract): boolean => {
		return VideoServiceHelper.autoplayServices.includes(pv.service);
	};

	// TODO: Test.
	static primaryPV = (
		pvs: PVContract[],
		preferredService?: PVService,
		autoplay?: boolean
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
						(p): boolean => p.pvType === PVType.Original,
						(p): boolean => p.pvType === PVType.Reprint,
					]
				) ??
				VideoServiceHelper.getPV(p, true, [
					(p): boolean => p.pvType === PVType.Original,
					(p): boolean => p.pvType === PVType.Reprint,
				])
			);
		} else {
			return VideoServiceHelper.getPV(p, true, [
				(p): boolean => p.pvType === PVType.Original,
				(p): boolean => p.pvType === PVType.Reprint,
			]);
		}
	};
}
