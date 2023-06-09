import { PVService } from '@/types/Models/PVs/PVService';
import { PVType } from '@/types/Models/PVs/PVType';

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
