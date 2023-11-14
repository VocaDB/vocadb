"use client"

import { LucidePlus } from "lucide-react"

import { buttonVariants } from "../ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"

export function AddEntryButton() {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <div className={buttonVariants({ variant: "default", size: "icon" })}>
          <span className="sr-only">Add entry</span>
          <LucidePlus className="h-5 w-5" />
        </div>
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Create entry</DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem>Artist</DropdownMenuItem>
        <DropdownMenuItem>Album</DropdownMenuItem>
        <DropdownMenuItem>Event</DropdownMenuItem>
        <DropdownMenuItem>Song</DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
