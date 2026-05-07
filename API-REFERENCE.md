# EventHub API — Frontend reference

**Base URL:** Configure per environment (e.g. `https://localhost:7xxx` in development). All paths below are relative to that base.

**Authentication:** JWT Bearer (`Authorization: Bearer <token>`). Issued by `POST /api/auth/login` and `POST /api/auth/register`. Enum values in JSON are serialized as strings (`UserRole`, `AccountStatus`, etc.).

---

## Auth — `api/auth`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| POST | `/api/auth/register` | No | JSON `RegisterRequest`: `email`, `password`, `firstName`, `lastName`, `applyAs` (`Admin` \| `EventOrganizer` \| `Participant`), optional `phoneNumber`. Returns `AuthResult` (`token`, `expiresAtUtc`, `user`). |
| POST | `/api/auth/login` | No | JSON `LoginRequest`: `email`, `password`. Returns `AuthResult`. |
| POST | `/api/auth/logout` | No | No body. Returns `204 No Content`. |
| POST | `/api/auth/reset-password` | **Yes** (any authenticated user) | JSON `ResetPasswordRequest`: `currentPassword`, `newPassword`. Returns `204 No Content`. |

---

## Events — `api/event`

| Method | Path | Auth | Body / query |
|--------|------|------|----------------|
| GET | `/api/event` | No | List all events. |
| GET | `/api/event/approved` | No | Approved events only. |
| GET | `/api/event/pending` | No | Pending events. |
| GET | `/api/event/upcoming` | No | Query: `count` (int, default `10`). |
| GET | `/api/event/search` | No | Query: `keyword`, `venue`, `categoryId`, `eventDate` (optional filters). |
| GET | `/api/event/{id}` | No | Single event by id. |
| GET | `/api/event/{eventId}/analytics` | **Yes**, role **`EventOrganizer`** | Organizer analytics for that event. |
| GET | `/api/event/organizer/{organizerId}` | No | Events by organizer. |
| GET | `/api/event/category/{categoryId}` | No | Events by category. |
| POST | `/api/event` | No | JSON `Event` entity (create). Returns `201` + `Location` header. |
| PUT | `/api/event/{id}` | No | JSON `Event`; body `id` must match route `id`. |
| DELETE | `/api/event/{id}` | No | Delete event. |

---

## Categories — `api/category`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/category` | No | All categories. |
| GET | `/api/category/with-counts` | No | Categories with event counts. |
| GET | `/api/category/{id}` | No | By id. |
| GET | `/api/category/name/{name}` | No | By name (URL-encoded). |
| POST | `/api/category` | No | JSON `Category`. Returns `201`. |
| PUT | `/api/category/{id}` | No | JSON `Category`; body id must match route. |
| DELETE | `/api/category/{id}` | No | |

---

## Users — `api/user`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/user/{id}` | No | User by id. |
| GET | `/api/user/email/{email}` | No | User by email (encode email in path). |
| GET | `/api/user/email-exists/{email}` | No | Returns boolean. |
| PUT | `/api/user/{id}` | No | JSON `User`; body id must match route. |
| DELETE | `/api/user/{id}` | No | |

---

## Tickets — `api/ticket`

| Method | Path | Auth | Body / query |
|--------|------|------|----------------|
| GET | `/api/ticket/{id}` | No | Ticket by id. |
| GET | `/api/ticket/qrcode/{qrCode}` | No | Lookup by QR code (encode if needed). |
| GET | `/api/ticket/participant/{participantId}` | No | Tickets for participant. |
| GET | `/api/ticket/event/{eventId}` | No | Tickets for event. |
| GET | `/api/ticket/participant/{participantId}/has-purchased/{eventId}` | No | Returns boolean. |
| POST | `/api/ticket/book` | No | Query: `eventId`, `participantId`. Books a ticket; returns `201`. |

---

## Reviews — `api/review`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| POST | `/api/review` | No | JSON `Review`. Returns `201`. |
| GET | `/api/review/event/{eventId}` | No | Reviews for event. |
| DELETE | `/api/review/{id}` | No | |

---

## Favorites — `api/favorite`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| POST | `/api/favorite` | No | JSON `Favorite`. Returns `201`. |
| GET | `/api/favorite/user/{userId}` | No | User’s favorites. |
| DELETE | `/api/favorite/{id}` | No | Remove favorite by id. |

---

## Attachments — `api/attachment`

| Method | Path | Auth | Body / query |
|--------|------|------|----------------|
| POST | `/api/attachment/upload` | No | `multipart/form-data`: file field + query `eventId`. Max request size ~50 MB. |
| GET | `/api/attachment/event/{eventId}` | No | Attachments for event. |
| DELETE | `/api/attachment/{id}` | No | |

---

## Notifications — `api/notification`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/notification/user/{userId}` | No | Notifications for user. |
| POST | `/api/notification/send` | No | JSON `Notification`. |
| PUT | `/api/notification/{id}/read` | No | Mark as read. Returns `204`. |

---

## Admin — `api/admin`

| Method | Path | Auth | Notes |
|--------|------|------|-------|
| POST | `/api/admin/organizers/{id}/approve` | *Not enforced in controller* | Approve organizer. `204`. |
| POST | `/api/admin/organizers/{id}/reject` | *Not enforced in controller* | Reject organizer. `204`. |
| POST | `/api/admin/events/{id}/approve` | *Not enforced in controller* | Approve event. `204`. |
| POST | `/api/admin/events/{id}/reject` | *Not enforced in controller* | Reject event. `204`. |

*Coordinate with backend if these routes should require an `Admin` JWT; the codebase may add `[Authorize]` later.*

---

## OpenAPI (development)

With the API running in **Development**, OpenAPI JSON is mapped (see `Program.cs` — `MapOpenApi()`). Use it for machine-readable schemas alongside this document.

---

## Summary counts

| Area | Endpoints |
|------|-----------|
| Auth | 4 |
| Events | 13 |
| Categories | 7 |
| Users | 5 |
| Tickets | 6 |
| Reviews | 3 |
| Favorites | 3 |
| Attachments | 3 |
| Notifications | 3 |
| Admin | 4 |
| **Total** | **51** |

---
