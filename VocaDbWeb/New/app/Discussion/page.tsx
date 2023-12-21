"use client"
import { apiFetch } from "@/client/fetch"
import { useEffect } from "react"

export default function DiscussionPage() {

  useEffect(() => {
    const run = async () => {
      await apiFetch("/antiforgery/token", {
      })
      await apiFetch("/users/login", {
        credentials: true,
        method: "POST",
        mode: "cors",
      })
    }

    run()
  }, [])

  return <></>
}
