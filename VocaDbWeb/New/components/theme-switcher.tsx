"use client"

import * as React from "react"

import { useConfig } from "@/hooks/use-config"

export function ThemeSwitcher() {
  const [config] = useConfig()

  React.useEffect(() => {
    document.body.classList.forEach((className) => {
      if (className.match(/^theme.*/)) {
        document.body.classList.remove(className)
      }
    })

    return document.body.classList.add(`theme-${config.theme}`)
  }, [config])

  return null
}
