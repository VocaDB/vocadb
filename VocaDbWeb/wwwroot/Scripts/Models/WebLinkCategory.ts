enum WebLinkCategory {
  Official,
  Commercial,
  Reference,
  Other,
}

export default WebLinkCategory;

export function parseWebLinkCategory(rating: string): WebLinkCategory {
  switch (rating) {
    case 'Official':
      return WebLinkCategory.Official;
    case 'Commercial':
      return WebLinkCategory.Commercial;
    case 'Reference':
      return WebLinkCategory.Reference;
    case 'Other':
      return WebLinkCategory.Other;
  }
}
