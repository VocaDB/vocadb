// Corresponds to the TagCategoryForApiContract class in C#.
export interface TagCategoryContract {
	name: string;
	tags: {
		additionalNames?: string;
		id: number;
		name: string;
		usageCount: number;
	}[];
}
