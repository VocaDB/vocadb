"use client"

import { usePathname } from "next/navigation"

import { cn } from "@/lib/utils"

import { Icons } from "./icons"

export interface SidebarNavItem {
  title: string
  href?: string
  disabled?: boolean
  external?: boolean
  icon?: keyof typeof Icons
  label?: string
  items?: SidebarNavItem[]
}

export interface SidebarNavProps {
  items: SidebarNavItem[]
}

export function SidebarNav({ items }: SidebarNavProps) {
  const pathname = usePathname()

  return items.length ? (
    <div className="w-full">
      {items.map((item, index) => (
        <div key={index} className={cn("pb-4")}>
          <h4 className="mb-1 px-2 py-1 text-sm font-semibold ">
            {item.title}
          </h4>
          {item.items?.length && (
            <SidebarNavItems items={item.items} pathname={pathname} />
          )}
        </div>
      ))}
    </div>
  ) : null
}

interface SidebarNavItemsProps {
  items?: SidebarNavItem[]
  pathname: string | null
}

export function SidebarNavItems({ items, pathname }: SidebarNavItemsProps) {
  return items?.length ? (
    <div className="grid grid-flow-row auto-rows-max text-sm">{}</div>
  ) : null
}
