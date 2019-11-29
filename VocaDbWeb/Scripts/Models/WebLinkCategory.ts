
//module vdb.models {

    export enum WebLinkCategory {
        Official,
        Commercial,
        Reference,
        Other
    }

    export function parseWebLinkCategory(rating: string) {

        switch (rating) {
            case "Official": return WebLinkCategory.Official;
            case "Commercial": return WebLinkCategory.Commercial;
            case "Reference": return WebLinkCategory.Reference;
            case "Other": return WebLinkCategory.Other;
        }

    }

//}