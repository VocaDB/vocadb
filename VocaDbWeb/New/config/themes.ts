import { StaticImageData } from "next/image"
import kagamine from "@/public/characters/kagamine.png"
import miku from "@/public/characters/miku.png"

export interface ColorTheme {
  name: string
  id: Theme
  image: StaticImageData
}

export const colorThemes = [
  {
    name: "Hatsune Miku",
    id: "miku",
    image: miku,
  },
  {
    name: "Kagamine Rin",
    id: "kagamine",
    image: kagamine,
  },
] as readonly ColorTheme[]

// TODO: Remove "zinc"
export type Theme = "zinc" | "miku" | "kagamine"
