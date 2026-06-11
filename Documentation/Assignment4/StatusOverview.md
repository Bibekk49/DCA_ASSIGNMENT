# Assignment 4 — Domain Model Implementation

## Base Classes

| Item | Status |
|------|--------|
| `ValueObject` abstract base (structural equality) | ✅ |
| `EntityBase<TId>` base class with typed ID | ✅ |

## Value Objects

| Value Object | Validation Rules | Status |
|---|---|---|
| `EventId` | GUID wrapper; `Reconstitute(Guid)` factory for EFC | ✅ |
| `EventTitle` | 3–75 characters, non-null | ✅ |
| `EventDescription` | 0–250 characters; empty string valid | ✅ |
| `EventMaxGuests` | 5–50 (min=5, max=50 per UC7 spec) | ✅ |
| `EventTimes` | Start ≥ 08:00, duration 1–10 h, start in future, same-day or overnight | ✅ |

## UC1 — Create Event

| Scenario | Description | Status |
|---|---|---|
| S1 | New event created with DRAFT status | ✅ |
| S2 | Default title "Working Title" | ✅ |
| S3 | Default max guests = 5 | ✅ |
| S4 | Default visibility = PRIVATE, times = null | ✅ |

## UC2 — Update Title

| Scenario | Description | Status |
|---|---|---|
| S1 | Valid title accepted in DRAFT | ✅ |
| S2 | READY event reverts to DRAFT on title change | ✅ |
| F1 | Title too short (< 3 chars) | ✅ |
| F2 | Title too long (> 75 chars) | ✅ |
| F3 | Title is null or empty | ✅ |
| F4 | Event is ACTIVE, COMPLETED, or CANCELLED | ✅ |

## UC3 — Update Description

| Scenario | Description | Status |
|---|---|---|
| S1 | Valid description accepted in DRAFT | ✅ |
| S2 | Empty string clears description | ✅ |
| S3 | READY event reverts to DRAFT on description change | ✅ |
| F1 | Description exceeds 250 characters | ✅ |
| F2 | Event is ACTIVE or COMPLETED | ✅ |
| F3 | Event is CANCELLED | ✅ |

## UC4 — Update Times

| Scenario | Description | Status |
|---|---|---|
| S1 | Valid same-day times accepted | ✅ |
| S2 | Valid overnight times accepted (end date = start + 1) | ✅ |
| S3 | READY event reverts to DRAFT on times change | ✅ |
| S4 | Start time exactly 08:00 accepted | ✅ |
| S5 | Duration exactly 1 h and 10 h accepted | ✅ |
| F1 | Start time before 08:00 | ✅ |
| F2 | Duration less than 1 hour | ✅ |
| F3 | Duration more than 10 hours | ✅ |
| F4 | Start time is in the past | ✅ |
| F5 | Same-day: end time ≤ start time | ✅ |
| F6 | Overnight: end time > 01:00 | ✅ |
| F7 | Event is ACTIVE or COMPLETED | ✅ |
| F8 | Event is CANCELLED | ✅ |

## UC5 — Make Event Public

| Scenario | Description | Status |
|---|---|---|
| S1 | Visibility set to PUBLIC in DRAFT, READY, or ACTIVE | ✅ |
| F1 | Event is CANCELLED | ✅ |

## UC6 — Make Event Private

| Scenario | Description | Status |
|---|---|---|
| S1 | Visibility set to PRIVATE in DRAFT or READY | ✅ |
| F1 | Event is ACTIVE | ✅ |
| F2 | Event is CANCELLED | ✅ |

## UC7 — Set Max Guests

| Scenario | Description | Status |
|---|---|---|
| S1 | Valid value (5–50) accepted in DRAFT | ✅ |
| S2 | READY event reverts to DRAFT on change | ✅ |
| S3 | ACTIVE event: increase allowed | ✅ |
| F1 | ACTIVE event: reduction blocked (`CannotReduceWhenActive`) | ✅ |
| F2 | Event is CANCELLED | ✅ |
| F3 | Event is COMPLETED | ✅ |
| F4 | Value below 5 (tried: 4, 1, 0, -1) | ✅ |
| F5 | Value above 50 (tried: 51, 100) | ✅ |

## UC8 — Ready Event

| Scenario | Description | Status |
|---|---|---|
| S1 | DRAFT event with valid title + times → READY | ✅ |
| F1 | Title is still "Working Title" | ✅ |
| F1b | Times not set | ✅ |
| F2 | Event is CANCELLED | ✅ |
| F3 | Start time is in the past at time of readying | ✅ |
| F4 | Event is ACTIVE or COMPLETED | ✅ |

## UC9 — Activate Event

| Scenario | Description | Status |
|---|---|---|
| S1 | DRAFT event auto-readied then ACTIVE | ✅ |
| S2 | READY event activated directly | ✅ |
| S3 | Already ACTIVE — idempotent success | ✅ |
| F1 | DRAFT fails ready check (title / times / past start) | ✅ |
| F2 | Event is CANCELLED | ✅ |
| F3 | Event is COMPLETED | ✅ |