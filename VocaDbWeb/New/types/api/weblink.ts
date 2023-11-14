export interface WebLinkContract {
  category: WebLinkCategory
  description: string
  descriptionOrUrl?: string
  disabled: boolean
  id: number
  url: string
}

export enum WebLinkCategory {
  Official = "Official",
  Commercial = "Commercial",
  Reference = "Reference",
  Other = "Other",
}
