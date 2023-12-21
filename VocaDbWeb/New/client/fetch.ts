export function authApiGet<T>(
  path: string,
  options?: ApiFetchOptions
): Promise<T> {
  return apiGet(path, { credentials: true, ...options })
}

export function apiGet<T>(path: string, options?: ApiFetchOptions): Promise<T> {
  return apiFetch(path, options).then((resp) => resp.json())
}

interface ApiFetchOptions {
  credentials?: boolean,
  method?: "GET" | "POST",
  mode?: RequestMode,
}
/**  If an endpoint returns JSON Data, apiGet should be used instead. */
export const apiFetch = async (
  path: string,
  options?: ApiFetchOptions
): Promise<Response> => {
  const resp = await fetch(
    process.env.NEXT_PUBLIC_API_URL + path,
    { credentials: options?.credentials ? "include" : undefined, method: options?.method, mode: options?.mode, headers: { "Content-Type": "application/json" } }
  )

  if (!resp.ok) {
    return Promise.reject(resp)
  }

  return resp
}
