"use client"

import Link from "next/link"
import {
  LucideBookOpen,
  LucideCalendar,
  LucideDisc,
  LucideHelpCircle,
  LucideIcon,
  LucideListMusic,
  LucideMedal,
  LucideMessageSquare,
  LucideMessagesSquare,
  LucideMic,
  LucideMic2,
  LucideMusic,
  LucidePlayCircle,
  LucideProps,
  LucideTag,
} from "lucide-react"

import { cn } from "@/lib/utils"

import { Button } from "./ui/button"
import { ScrollArea } from "./ui/scroll-area"

interface SidebarProps extends React.HTMLAttributes<HTMLDivElement> {
  playlists: Playlist[]
}

export type Playlist = (typeof playlists)[number]

export const playlists = [
  "Recently Added",
  "Recently Played",
  "Top Songs",
  "Top Albums",
  "Top Artists",
  "Logic Discography",
  "Bedtime Beats",
  "Feeling Happy",
  "I miss Y2K Pop",
  "Runtober",
  "Mellow Days",
  "Eminem Essentials",
]

interface NavLinkProps {
  name: string
  href: string
  icon: JSX.Element
}

export function NavLink({ icon, name, href }: NavLinkProps) {
  return (
    <Button variant="ghost" className="w-full justify-start" asChild>
      <Link href={href}>
        {icon}
        {name}
      </Link>
    </Button>
  )
}

interface TopicProps extends React.HTMLAttributes<HTMLDivElement> {
  topic: string
  links: NavLinkProps[]
}

export function Topic({ className, topic, links }: TopicProps) {
  return (
    <div className={cn("px-3 py-2", className)}>
      <h2 className="mb-2 px-4 text-lg font-semibold tracking-tight">
        {topic}
      </h2>
      {links.map((link, i) => (
        <NavLink {...link} key={i} />
      ))}
    </div>
  )
}

export function Sidebar({ className, playlists }: SidebarProps) {
  return (
    <ScrollArea className="h-[calc(100vh-4rem)] border-r">
      <div className={cn("pb-12", className)}>
        <div className="space-y-4 py-4">
          <Topic
            topic="Discover"
            links={[
              {
                href: "/Search?searchType=Song&sort=AdditionDate&onlyWithPVs=true",
                name: "Recent PVs",
                icon: <LucidePlayCircle className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Song/Rankings?dateFilterType=CreateDate&durationHours=168",
                name: "Top rated songs",
                icon: <LucideMedal className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Search?searchType=Album&sort=RatingAverage",
                name: "Top rated albums",
                icon: <LucideDisc className="mr-2 h-4 w-4" />,
              },
              {
                href: "/SongList/Featured",
                name: "Featured song lists",
                icon: <LucideListMusic className="mr-2 h-4 w-4" />,
              },
            ]}
          />
          <Topic
            topic="Library"
            links={[
              {
                href: "/Search?searchType=Song",
                name: "Songs",
                icon: <LucideMusic className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Search?searchType=Artist",
                name: "Artists",
                icon: <LucideMic2 className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Search/?searchType=Albums",
                name: "Albums",
                icon: <LucideDisc className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Search?searchType=ReleaseEvent",
                name: "Events",
                icon: <LucideCalendar className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Tag",
                name: "Tags",
                icon: <LucideTag className="mr-2 h-4 w-4" />,
              },
            ]}
          />
          <Topic
            topic="Documentation"
            links={[
              {
                href: "Discussion",
                name: "Discussions",
                icon: <LucideMessagesSquare className="mr-2 h-4 w-4" />,
              },
              {
                href: "/Help",
                name: "Help / About",
                icon: <LucideHelpCircle className="mr-2 h-4 w-4" />,
              },
              {
                href: "//wiki.vocadb.net/",
                name: "Wiki",
                icon: <LucideBookOpen className="mr-2 h-4 w-4" />,
              },
            ]}
          />
        </div>
      </div>
    </ScrollArea>
  )
}
