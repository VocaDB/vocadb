import { useAtom } from "jotai"
import { atomWithStorage } from "jotai/utils"

import { Theme } from "@/config/themes"

type ThemeConfig = {
  theme: Theme
}

const themeConfigAtom = atomWithStorage<ThemeConfig>("themeConfig", {
  theme: "zinc",
})

export function useConfig() {
  return useAtom(themeConfigAtom)
}
