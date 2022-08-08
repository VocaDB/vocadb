import WebLinkCategory from '@/Models/WebLinkCategory';

export default interface WebLinkContract {
	category: WebLinkCategory;

	description: string;

	descriptionOrUrl?: string;

	disabled: boolean;

	id: number;

	url: string;
}
