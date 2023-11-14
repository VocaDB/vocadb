import Link from "next/link"
import { SiDiscord, SiTwitter } from "@icons-pack/react-simple-icons"

import { siteConfig } from "@/config/site"
import { buttonVariants } from "@/components/ui/button"
import { MainNav } from "@/components/main-nav"
import { ThemeToggle } from "@/components/theme-toggle"

import { AddEntryButton } from "./buttons/add-entry"
import { ColorThemeSwitcher } from "./color-menu"

interface SocialLinkProps {
  href: string
  name: string
  icon: JSX.Element
}

export function SocialLink({ href, name, icon }: SocialLinkProps) {
  return (
    <Link href={href} target="_blank">
      <div className={buttonVariants({ variant: "ghost", size: "icon" })}>
        {icon}
        <span className="sr-only">{name}</span>
      </div>
    </Link>
  )
}

export function SiteHeader() {
  return (
    <header className="bg-background sticky top-0 z-40 w-full border-b">
      <div className="container flex h-16 items-center space-x-4 sm:justify-between sm:space-x-0">
        <MainNav items={siteConfig.mainNav} />
        <div className="flex grow justify-center">
          <input placeholder="Search" />
        </div>
        <div className="flex flex-1 items-center justify-end space-x-4">
          <AddEntryButton />
          <nav className="flex items-center space-x-1">
            <SocialLink
              icon={<SiDiscord />}
              name="Discord"
              href="//discord.com/invite/3bwXQNXKCz"
            />
            <SocialLink
              icon={<SiTwitter className="h-5 w-5" />}
              name="Twitter"
              href="//twitter.com/vocadb"
            />
            <ColorThemeSwitcher />
            <ThemeToggle />
          </nav>
        </div>
      </div>
    </header>
  )
}
