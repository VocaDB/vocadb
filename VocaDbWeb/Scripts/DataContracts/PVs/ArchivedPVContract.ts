import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';

export interface ArchivedPVContract {
	author: string;
	disabled: boolean;
	length: number;
	name: string;
	publishDate?: string;
	pvId: string;
	pvType: PVType;
	service: PVService;
	thumbUrl: string;
}
