import {
	BandcampPVContract,
	PiaproPVContract,
	PVContract,
	SoundCloudPVContract,
} from '@/DataContracts/PVs/PVContract';
import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';
import { functions } from '@/Shared/GlobalFunctions';
import _ from 'lodash';

interface BandcampMetadata {
	Url?: string;
}

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

	public static getPiaproTimestamp = (
		pv: PiaproPVContract,
	): string | undefined => {
		const meta =
			pv.extendedMetadata && pv.extendedMetadata.json
				? (JSON.parse(pv.extendedMetadata.json) as PiaproMetadata)
				: undefined;

		return meta?.Timestamp;
	};

	private static getPiaproUrlWithTimestamp = (
		pv: PiaproPVContract,
	): string | undefined => {
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

	public static getVideoId = (pv: PVContract): string | undefined => {
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

	public static getUrlById = (pv: PVContract): string => {
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
				return functions.mergeUrls(
					vdb.values.staticContentHost,
					`/media/${pv.pvId}`,
				);

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

	public static readonly autoplayServices = [
		PVService.File,
		PVService.LocalFile,
		PVService.NicoNicoDouga,
		PVService.Piapro,
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
						(p): boolean => p.pvType === PVType.Original,
						(p): boolean => p.pvType === PVType.Reprint,
					],
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
