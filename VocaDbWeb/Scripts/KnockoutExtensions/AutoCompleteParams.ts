
module vdb.knockoutExtensions {

    export interface AutoCompleteParams {

		acceptSelection?: (id: number, term: string, itemType?: string) => void;

		allowCreateNew?: boolean;

		createCustomItem?: string;

		createNewItem?: string;

		extraQueryParams?: any;

		filter?: (any) => boolean;

		height?: number;

		// Ignore entry by this ID
		ignoreId?: number;

    }

}

