// Corresponds to the TagCategoryForApiContract class in C#.
export default interface TagCategoryContract {
	name: string;
	tags: {
		additionalNames?: string;
		id: number;
		name: string;
		usageCount: number;
	}[];
}
