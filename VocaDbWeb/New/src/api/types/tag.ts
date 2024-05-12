export interface TagUsage {
	count: number;
	tag: Tag;
}

export interface Tag {
	additionalNames: string;
	categoryName: string;
	id: number;
	name: string;
	urlSlug: string;
}
