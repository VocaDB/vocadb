import { WebLinkCategory } from '@/Models/WebLinkCategory';

export interface ArchivedWebLinkContract {
	category: WebLinkCategory;
	description: string;
	disabled: boolean;
	url: string;
}
