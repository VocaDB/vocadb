import { PVExtendedMetadata } from '@/DataContracts/PVs/PVContract';
import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';

interface ArchivedPVContractBase {
	author: string;
	disabled: boolean;
	length: number;
	name: string;
	publishDate?: string;
	pvId: string;
	pvType: PVType;
	thumbUrl: string;
}

export interface ArchivedBandcampPVContract extends ArchivedPVContractBase {
	service: PVService.Bandcamp;
	extendedMetadata?: PVExtendedMetadata;
}

interface DefaultArchivedPVContract extends ArchivedPVContractBase {
	service: Exclude<PVService, ArchivedBandcampPVContract['service']>;
}

export type ArchivedPVContract =
	| ArchivedBandcampPVContract
	| DefaultArchivedPVContract;
