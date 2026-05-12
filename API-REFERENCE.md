# EventHub API — Frontend reference

**Base URL:** Configure per environment (e.g. `https://localhost:7xxx` in development). All paths below are relative to that base.

**Authentication:** JWT Bearer (`Authorization: Bearer <token>`). Issued by `POST /api/auth/login` and `POST /api/auth/register`. The API uses a **default authorization policy** (`Program.cs`) that requires an authenticated user unless an endpoint is explicitly anonymous (`[AllowAnonymous]`). Enum values in JSON are serialized as strings (`UserRole`, `AccountStatus`, etc.).

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
| GET | `/api/event` | No | List all events. Response items are `EventResponseDto` (includes nested organizer/category summaries, attachments, reviews where populated). |
| GET | `/api/event/approved` | **Yes**, role **`EventOrganizer`** | Approved events for the authenticated organizer. |
| GET | `/api/event/pending` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | Pending events: admin sees all pending; organizer sees only their own. |
| GET | `/api/event/upcoming` | No | Query: `count` (int, default `10`). |
| GET | `/api/event/search` | No | Query: `keyword`, `venue`, `categoryId`, `eventDate` (optional filters). |
| GET | `/api/event/{id}` | No | Single event by id (`EventResponseDto`). |
| GET | `/api/event/{eventId}/analytics` | **Yes**, role **`EventOrganizer`** | Organizer analytics for that event (caller must own the event). |
| GET | `/api/event/organizer/{organizerId}` | No | Events by organizer. |
| GET | `/api/event/category/{categoryId}` | No | Events by category. |
| POST | `/api/event` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | JSON `EventCreateDto`: `categoryId`, `title`, `description`, `venue`, `eventDate`, optional `image`, `price`, `totalTickets`, `availableTickets`. Admins must also send `organizerId`; organizers omit it (organizer is inferred from the JWT). Returns `201` + `Location`; body is `EventResponseDto`. |
| PUT | `/api/event/{id}` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | JSON `EventUpdateDto`: same shape as create except no `organizerId`. Caller must be allowed to mutate that event (admin or owning organizer). Returns `204 No Content`. |
| DELETE | `/api/event/{id}` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | Same mutation rules as PUT. Returns `204 No Content`. |

---

## Categories — `api/category`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/category` | No | All categories. |
| GET | `/api/category/{id}` | **Yes**, role **`Admin`** | By id. |
| GET | `/api/category/name/{name}` | No | By name (URL-encoded). |
| POST | `/api/category` | **Yes**, role **`Admin`** | JSON `Category`. Returns `201`. |
| PUT | `/api/category/{id}` | **Yes**, role **`Admin`** | JSON `Category`; body id must match route. |
| DELETE | `/api/category/{id}` | **Yes**, role **`Admin`** | |

---

## Users — `api/user`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/user/{id}` | **Yes** | User by id. |
| GET | `/api/user/email/{email}` | **Yes** | User by email (encode email in path). |
| GET | `/api/user/email-exists/{email}` | **Yes** | Returns boolean. |
| PUT | `/api/user/{id}` | **Yes** | JSON `User`; body id must match route. |
| DELETE | `/api/user/{id}` | **Yes** | |

---

## Tickets — `api/ticket`

| Method | Path | Auth | Body / query |
|--------|------|------|----------------|
| GET | `/api/ticket/verify/{qrCode}` | No | Public QR verification; **first successful check marks the ticket as used**. `200` returns verification payload (`ticketId`, `qrCode`, `eventId`, `eventTitle`, `eventDate`, `venue`, `participantFullName`, `purchasedAt`, `verifiedAtUtc`). `404` / `400` return `{ "message": "..." }` for invalid or already-used tickets. |
| GET | `/api/ticket/{id}/qr-image` | **Yes** | PNG file for the ticket QR image. Allowed if caller owns the ticket as **`Participant`**, is **`Admin`**, or is the **`EventOrganizer`** for that event. |
| GET | `/api/ticket/{id}` | **Yes** | Ticket by id. |
| GET | `/api/ticket/qrcode/{qrCode}` | **Yes** | Lookup by QR code (encode if needed). |
| GET | `/api/ticket/participant/{participantId}` | **Yes** | Tickets for participant. |
| GET | `/api/ticket/event/{eventId}` | **Yes** | Tickets for event. |
| GET | `/api/ticket/participant/{participantId}/has-purchased/{eventId}` | **Yes** | Returns boolean. |
| POST | `/api/ticket/purchase/{eventId}` | **Yes**, role **`Participant`** | No body. Participant id comes from the JWT. Returns `200` + JSON `TicketPurchaseResult`: `orderId`, `eventId`, `participantId`, `totalPrice`, `remainingAvailableTickets`, `ticketId`, `ticketQrCode`, `ticketQrCodeImagePath`. |

---

## Orders — `api/order`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/order/{orderId}` | **Yes**, **`Participant`** (own order only) or **`Admin`** | `OrderDetailsDto`: `id`, `participantId`, `eventId`, `totalPrice`, `createdAt`, nested `event` (`OrderEventDto`), `tickets` (`OrderTicketDto` list). |
| GET | `/api/order/my-orders` | **Yes**, role **`Participant`** | List of `OrderDetailsDto` for the authenticated participant. |
| GET | `/api/order/event/{eventId}` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | Orders for that event; organizers must be allowed to manage that event. List of `OrderDetailsDto`. |

---

## Reviews — `api/review`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| POST | `/api/review` | **Yes**, role **`Participant`** | JSON `CreateReviewRequest`: `eventId`, `rating` (1–5), optional `comment`. Returns `201`. |
| GET | `/api/review/event/{eventId}` | No | Reviews for event. |
| DELETE | `/api/review/{id}` | **Yes**, role **`Participant`** | Deletes own review (authorization enforced server-side). |

---

## Favorites — `api/favorite`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| POST | `/api/favorite/event/{eventId}` | **Yes**, role **`Participant`** | No body; user comes from JWT. Returns `201` + `Location`. |
| GET | `/api/favorite/user/{userId}` | **Yes**, role **`Admin`** | User’s favorites. |
| DELETE | `/api/favorite/{id}` | **Yes**, role **`Participant`** | Remove favorite by id (must belong to caller). |

---

## Attachments — `api/attachment`

| Method | Path | Auth | Body / query |
|--------|------|------|----------------|
| POST | `/api/attachment/upload` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | `multipart/form-data`: file field + query `eventId`. Organizer must be allowed to mutate that event. Max request size ~50 MB. Returns `200` + `EventAttachmentResponseDto` (`id`, `eventId`, `filePath`, `createdAt`). |
| GET | `/api/attachment/event/{eventId}` | No | Attachments for event (`EventAttachmentResponseDto` list). |
| DELETE | `/api/attachment/{id}` | **Yes**, roles **`Admin`** or **`EventOrganizer`** | Organizer must be allowed to mutate the related event. |

---

## Notifications — `api/notification`

| Method | Path | Auth | Body / notes |
|--------|------|------|----------------|
| GET | `/api/notification/user/{userId}` | **Yes** | Notifications for user; caller must be that user or **`Admin`**. |
| POST | `/api/notification/send` | **Yes**, role **`Admin`** | JSON `Notification`. |
| PUT | `/api/notification/{id}/read` | **Yes**, role **`Participant`** | Mark as read for the authenticated participant. Returns `204`. |

---

## Admin — `api/admin`

| Method | Path | Auth | Notes |
|--------|------|------|-------|
| POST | `/api/admin/organizers/{id}/approve` | **Yes**, role **`Admin`** | Approve organizer. `204`. |
| POST | `/api/admin/organizers/{id}/reject` | **Yes**, role **`Admin`** | Reject organizer. `204`. |
| POST | `/api/admin/events/{id}/approve` | **Yes**, role **`Admin`** | Approve event. `204`. |
| POST | `/api/admin/events/{id}/reject` | **Yes**, role **`Admin`** | Reject event. `204`. |

---

## OpenAPI (development)

With the API running in **Development**, OpenAPI JSON is mapped (see `Program.cs` — Swagger). Use it for machine-readable schemas alongside this document.

---

## Summary counts

| Area | Endpoints |
|------|-----------|
| Auth | 4 |
| Events | 13 |
| Categories | 6 |
| Users | 5 |
| Tickets | 8 |
| Orders | 3 |
| Reviews | 3 |
| Favorites | 3 |
| Attachments | 3 |
| Notifications | 3 |
| Admin | 4 |
| **Total** | **55** |

---