
//module vdb.models {

    enum WebLinkCategory {
        Official,
        Commercial,
        Reference,
        Other
    }

	export default WebLinkCategory;

    export function parseWebLinkCategory(rating: string) {

        switch (rating) {
            case "Official": return WebLinkCategory.Official;
            case "Commercial": return WebLinkCategory.Commercial;
            case "Reference": return WebLinkCategory.Reference;
            case "Other": return WebLinkCategory.Other;
        }

    }

//}