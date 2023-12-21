"use client"
import { apiFetch } from "@/client/fetch"
import { useState } from "react"

export default function DiscussionPage() {

  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  const run = async () => {
    await apiFetch("/antiforgery/token", {
    })
    await apiFetch("/users/login", {
      credentials: true,
      method: "POST",
      mode: "cors",
      body: JSON.stringify({
        keepLoggedIn: true,
        password,
        userName: email
      })
    })
  }


  return <>
    <input value={email} onChange={(e) => setEmail(e.target.value)} />
    <input value={password} onChange={(e) => setPassword(e.target.value)} />
    <button onClick={() => run()}>Send</button>
  </>
}
