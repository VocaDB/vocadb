import {
	ArchivedBandcampPVContract,
	ArchivedPVContract,
} from '@/DataContracts/PVs/ArchivedPVContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { PVService } from '@/Models/PVs/PVService';
import { functions } from '@/Shared/GlobalFunctions';
import moment from 'moment';
import React from 'react';

interface BandcampMetadata {
	Url?: string;
}

const getBandcampUrlById = (pv: ArchivedBandcampPVContract): string => {
	const bandcampMetadata =
		pv.extendedMetadata && pv.extendedMetadata.json
			? (JSON.parse(pv.extendedMetadata.json) as BandcampMetadata)
			: undefined;
	return bandcampMetadata?.Url ?? `https://bandcamp.com/track/${pv.pvId}`;
};

const getUrl = (pv: ArchivedPVContract): string => {
	switch (pv.service) {
		case PVService.Bandcamp:
			return getBandcampUrlById(pv);

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
			return `https://youtu.be/${pv.pvId}`;
	}
};

interface PVInfoProps {
	pv: ArchivedPVContract;
}

export const PVInfo = React.memo(
	({ pv }: PVInfoProps): React.ReactElement => {
		return (
			<>
				{pv.service}:{' '}
				<a href={getUrl(pv)} /* TODO: target="_blank" */>{pv.pvId}</a> by{' '}
				{pv.author} ({DateTimeHelper.formatFromSeconds(pv.length)})
				{pv.publishDate && <> at {moment(pv.publishDate).format('l')}</>}
				{pv.disabled && <> (unavailable)</>}
			</>
		);
	},
);
