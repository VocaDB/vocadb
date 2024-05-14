export enum WebLinkCategory {
	Official = "Official",
	Reference = "Reference",
	Commercial = "Commercial",
	Other = "Other",
}

export interface WebLink {
	category: WebLinkCategory;
	description: string;
	disabled: boolean;
	url: string;
}
