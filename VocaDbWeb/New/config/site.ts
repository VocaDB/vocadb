export type SiteConfig = typeof siteConfig

export const siteConfig = {
  name: "VocaDB",
  description:
    "VocaDB is a Vocaloid music database with translated artists, albums and songs.",
  mainNav: [
    {
      title: "Home",
      href: "/",
    },
  ],
  links: {
    twitter: "https://twitter.com/vocadb",
    github: "https://github.com/vocadb/vocadb",
    docs: "https://ui.shadcn.com",
  },
}
