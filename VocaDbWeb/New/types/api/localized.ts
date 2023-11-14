export interface LocalizedStringWithIdContract extends LocalizedStringContract {
  id: number
}

export interface LocalizedStringContract {
  language: ContentLanguageSelection
  value: string
}

export enum ContentLanguageSelection {
  Unspecified = "Unspecified",
  Japanese = "Japanese",
  Romaji = "Romaji",
  English = "English",
}
