"use client"

import Image from "next/image"
import { LucidePalette } from "lucide-react"

import { ColorTheme, colorThemes } from "@/config/themes"
import { cn } from "@/lib/utils"
import { useConfig } from "@/hooks/use-config"

import CustomImage from "./image"
import { Button } from "./ui/button"
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "./ui/card"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "./ui/dialog"

interface ColorCardProps {
  colorTheme: ColorTheme
}

export function ColorCard({ colorTheme }: ColorCardProps) {
  const [, setConfig] = useConfig()

  return (
    <Card className="w-fit">
      <CardHeader>
        <CardTitle>{colorTheme.name}</CardTitle>
      </CardHeader>
      <CardContent>
        <Image
          className="ml-auto mr-auto"
          height={120}
          src={colorTheme.image}
          alt={colorTheme.name}
        />
      </CardContent>
      <CardFooter>
        <Button
          onClick={() => setConfig({ theme: colorTheme.id })}
          className={cn("w-full", `theme-${colorTheme.id}`)}
        >
          Choose theme
        </Button>
      </CardFooter>
    </Card>
  )
}

export function ColorThemeSwitcher() {
  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button aria-label="Open Color Scheme Menu" variant="ghost" size="icon">
          <LucidePalette />
        </Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Change color palette</DialogTitle>
          <DialogDescription>
            Edit your preferred color scheme here. The moon and the sun
            indicate, if a color scheme is suitable for light/dark mode.
          </DialogDescription>
        </DialogHeader>
        <div className="flex space-x-4 justify-center">
          {colorThemes.map((theme) => (
            <ColorCard colorTheme={theme} key={theme.id} />
          ))}
        </div>
      </DialogContent>
    </Dialog>
  )
}
