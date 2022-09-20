import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';

interface PVContractBase {
	author?: string;
	createdBy?: number;
	disabled?: boolean;
	id: number;
	length?: number;
	name?: string;
	pvId: string;
	publishDate?: string;
	pvType: PVType;
	thumbUrl?: string;
	url?: string;
}

export interface PiaproPVContract extends PVContractBase {
	service: PVService.Piapro;
	extendedMetadata?: { json?: string };
}

interface DefaultPVContract extends PVContractBase {
	service: Exclude<PVService, PiaproPVContract['service']>;
}

export type PVContract = PiaproPVContract | DefaultPVContract;
