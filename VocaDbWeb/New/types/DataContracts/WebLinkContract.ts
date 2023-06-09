import { WebLinkCategory } from '@/types/Models/WebLinkCategory';

export interface WebLinkContract {
	category: WebLinkCategory;

	description: string;

	descriptionOrUrl?: string;

	disabled: boolean;

	id: number;

	url: string;
}
