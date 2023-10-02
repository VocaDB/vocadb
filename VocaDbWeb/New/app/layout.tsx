import "@/styles/globals.css"
import { Metadata } from "next"

import { siteConfig } from "@/config/site"
import { fontSans } from "@/lib/fonts"
import { cn } from "@/lib/utils"
import { SidebarNav } from "@/components/sidebar-nav"
import { SiteHeader } from "@/components/site-header"
import { TailwindIndicator } from "@/components/tailwind-indicator"
import { ThemeProvider } from "@/components/theme-provider"

export const metadata: Metadata = {
  title: {
    default: siteConfig.name,
    template: `%s - ${siteConfig.name}`,
  },
  description: siteConfig.description,
  themeColor: [
    { media: "(prefers-color-scheme: light)", color: "white" },
    { media: "(prefers-color-scheme: dark)", color: "black" },
  ],
  icons: {
    icon: "/favicon.ico",
    shortcut: "/favicon-16x16.png",
    apple: "/apple-touch-icon.png",
  },
}

interface RootLayoutProps {
  children: React.ReactNode
}

export default function RootLayout({ children }: RootLayoutProps) {
  return (
    <>
      <html lang="en" suppressHydrationWarning>
        <head />
        <body
          className={cn(
            "min-h-screen bg-background font-sans antialiased",
            fontSans.variable
          )}
        >
          <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
            <div className="relative flex min-h-screen flex-col">
              <SiteHeader />
              <div className="container flex-1 items-start md:grid md:grid-cols-[220px_minmax(0,1fr)] md:gap-6 lg:grid-cols-[240px_minmax(0,1fr)] lg:gap-10">
                <aside className="fixed top-12 z-30 -ml-2 hidden h-[calc(100vh-3.5rem)] w-full shrink-0 md:sticky md:block">
                  <SidebarNav
                    items={[
                      {
                        title: "Getting Started",
                        items: [
                          {
                            title: "Introduction",
                            href: "/docs",
                            items: [],
                          },
                          {
                            title: "Installation",
                            href: "/docs/installation",
                            items: [],
                          },
                          {
                            title: "components.json",
                            href: "/docs/components-json",
                            items: [],
                          },
                          {
                            title: "Theming",
                            href: "/docs/theming",
                            items: [],
                          },
                          {
                            title: "Dark mode",
                            href: "/docs/dark-mode",
                            items: [],
                          },
                          {
                            title: "CLI",
                            href: "/docs/cli",
                            items: [],
                          },
                          {
                            title: "Typography",
                            href: "/docs/components/typography",
                            items: [],
                          },
                          {
                            title: "Figma",
                            href: "/docs/figma",
                            items: [],
                          },
                          {
                            title: "Changelog",
                            href: "/docs/changelog",
                            items: [],
                          },
                          {
                            title: "About",
                            href: "/docs/about",
                            items: [],
                          },
                        ],
                      },
                      {
                        title: "Installation",
                        items: [
                          {
                            title: "Next.js",
                            href: "/docs/installation/next",
                            items: [],
                          },
                          {
                            title: "Vite",
                            href: "/docs/installation/vite",
                            items: [],
                          },
                          {
                            title: "Remix",
                            href: "/docs/installation/remix",
                            items: [],
                          },
                          {
                            title: "Gatsby",
                            href: "/docs/installation/gatsby",
                            items: [],
                          },
                          {
                            title: "Astro",
                            href: "/docs/installation/astro",
                            items: [],
                          },
                          {
                            title: "Laravel",
                            href: "/docs/installation/laravel",
                            items: [],
                          },
                          {
                            title: "Manual",
                            href: "/docs/installation/manual",
                            items: [],
                          },
                        ],
                      },
                    ]}
                  />
                </aside>
                <div>{children}</div>
              </div>
            </div>
            <TailwindIndicator />
          </ThemeProvider>
        </body>
      </html>
    </>
  )
}
