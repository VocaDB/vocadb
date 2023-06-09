import { WebLinkCategory } from '@/types/Models/WebLinkCategory';

export interface ArchivedWebLinkContract {
	category: WebLinkCategory;
	description: string;
	disabled: boolean;
	url: string;
}
