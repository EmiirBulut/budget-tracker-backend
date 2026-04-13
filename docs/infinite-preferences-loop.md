# Infinite GET Loop on Login Page — Root Cause & Fix

## Summary

When the frontend application loads (including on the `/login` page), it fires an
unguarded `GET /api/settings/preferences` request. Because the user is unauthenticated,
the backend returns **401 Unauthorised**. The axios response interceptor then attempts to
refresh the access token, fails (no refresh token present), calls `clearAuth()`, and
redirects the browser to `/login` via `window.location.replace`. The redirect causes a
full page reload, the app remounts, and the cycle repeats — producing a continuous stream
of `GET /api/settings/preferences` requests.

## Affected repos

| Repo | Role |
|------|------|
| `EmiirBulut/budget-tracker-frontend` | **Caller** — fires the unguarded query |
| `EmiirBulut/budget-tracker-backend`  | Responds correctly with 401; not the cause |

## Root cause (frontend)

`src/App.tsx` mounts `LanguageSync` unconditionally:

```tsx
// src/App.tsx
function LanguageSync() {
  const { data } = usePreferences()   // always runs, even on /login
  useEffect(() => {
    if (data) {
      const code = LANGUAGE_NAME_TO_CODE[data.language] ?? 'en'
      i18n.changeLanguage(code)
    }
  }, [data])
  return null
}

function AppProviders() {
  return (
    <QueryClientProvider client={queryClient}>
      <LanguageSync />          {/* rendered on every route */}
      <RouterProvider router={router} />
    </QueryClientProvider>
  )
}
```

`usePreferences` has no `enabled` guard:

```ts
// src/features/settings/hooks/usePreferences.ts
export function usePreferences() {
  return useQuery({
    queryKey: PREFERENCES_QUERY_KEY,
    queryFn: getPreferences,
    // ❌ no `enabled` check — fires even when unauthenticated
  })
}
```

The sequence that creates the loop:

```
Login page loads
  └─ LanguageSync mounts
       └─ usePreferences() → GET /api/settings/preferences
            └─ 401 Unauthorised (no token)
                 └─ axios interceptor: no refresh token → clearAuth() + location.replace('/login')
                      └─ Page reload → LanguageSync mounts again → loop
```

## Fix (frontend — `src/features/settings/hooks/usePreferences.ts`)

Gate the query behind the presence of an access token so it only runs when the user is
authenticated:

```ts
import { useQuery } from '@tanstack/react-query'
import { getPreferences } from '../api/settingsApi'
import { useAuthStore } from '../../auth/store/authStore'

export const PREFERENCES_QUERY_KEY = ['preferences'] as const

export function usePreferences() {
  const accessToken = useAuthStore((s) => s.accessToken)

  return useQuery({
    queryKey: PREFERENCES_QUERY_KEY,
    queryFn: getPreferences,
    enabled: !!accessToken,   // ✅ only fetch when authenticated
    retry: false,             // avoid hammering when logged out
  })
}
```

This single change stops the request storm on the login page. Once `accessToken` is set
(after a successful login), `enabled` becomes `true` and the query fires normally.

## Why 304 responses appeared

The user reported seeing HTTP **304 Not Modified** responses. This occurs because:

1. The browser previously received a successful `200 OK` for `/api/settings/preferences`
   (while logged in) and stored the response in its HTTP cache.
2. On subsequent requests the browser sends a conditional `If-None-Match` / `If-Modified-Since`
   header; if the access token is still valid (or the bearer token is attached from
   localStorage) the server validates the request and the browser serves the cached response
   (304), hiding the true status.

The `Cache-Control: no-store` response header added to the `GET /api/settings/preferences`
endpoint (see `SettingsController.cs`) prevents the browser from caching the response at
all, eliminating this confusion.



## Backend changes in this PR

The backend itself was **not** the cause of the loop. However two small improvements are
included:

1. **`Cache-Control: no-store`** — Added to `GET /api/settings/preferences` via
   `[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]`. This prevents
   browsers from caching authenticated API responses, which removes the 304 symptoms and
   is a good security practice for personalized data.

2. **`retry: false` guidance** — Documented above; must be applied in the frontend.
