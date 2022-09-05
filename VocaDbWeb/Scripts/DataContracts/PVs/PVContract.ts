import { PVService } from '@/Models/PVs/PVService';

export interface PVContract {
	author?: string;

	createdBy?: number;

	disabled?: boolean;

	extendedMetadata?: any;

	id?: number;

	length?: number;

	name?: string;

	pvId: string;

	service: PVService;

	publishDate?: string;

	pvType: string;

	thumbUrl?: string;

	url?: string;
}
