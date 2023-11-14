import Link from "next/link"
import { apiGet } from "@/client/fetch"

import { FrontPageContract } from "@/types/api/frontpage"
import { siteConfig } from "@/config/site"
import { buttonVariants } from "@/components/ui/button"
import { ColorThemeSwitcher } from "@/components/color-menu"

import { FeaturedSongsCarousel } from "../components/featured-songs"

async function getData() {
  const res = await apiGet<FrontPageContract>("/frontpage")

  return res
}

export default async function IndexPage() {
  const data = await getData()

  return (
    <section className="container grid items-center gap-6 pb-8 pt-6 md:py-10">
      <FeaturedSongsCarousel songs={data.newSongs} />
      <div className="flex max-w-[980px] flex-col items-start gap-2">
        <h1 className="text-3xl font-extrabold leading-tight tracking-tighter md:text-4xl">
          Beautifully designed components <br className="hidden sm:inline" />
          built with Radix UI and Tailwind CSS.
        </h1>
        <p className="max-w-[700px] text-lg text-muted-foreground">
          Accessible and customizable components that you can copy and paste
          into your apps. Free. Open Source. And Next.js 13 Ready.
        </p>
      </div>
      <div className="flex gap-4">
        <Link
          href={siteConfig.links.docs}
          target="_blank"
          rel="noreferrer"
          className={buttonVariants()}
        >
          Documentation
        </Link>
        <Link
          target="_blank"
          rel="noreferrer"
          href={siteConfig.links.github}
          className={buttonVariants({ variant: "outline" })}
        >
          GitHub
        </Link>
        <ColorThemeSwitcher />
      </div>
    </section>
  )
}
