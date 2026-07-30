# FitForge AI - API Specification

**Purpose:** Complete API contract. Frontend and backend teams implement against this document.

**Version:** 1.0
**Last Updated:** 2026-07-28

**Depends on:** DOMAIN_GLOSSARY.md, REQUIREMENTS.md

---

## 1. API Overview

### 1.1 Base URL

```
Development:  http://localhost:5000/api/v1
Staging:      https://staging-api.fitforge.ai/api/v1
Production:   https://api.fitforge.ai/api/v1
```

### 1.2 Versioning

- API version is in the URL path: `/api/v1/...`
- Breaking changes require a new version (`/api/v2/...`)
- Non-breaking changes (new fields, new endpoints) are added to the current version

### 1.3 Content Type

- Request: `Content-Type: application/json`
- Response: `Content-Type: application/json`

### 1.4 Authentication

- Bearer token in `Authorization` header: `Authorization: Bearer <accessToken>`
- Refresh token sent in request body or HttpOnly cookie

---

## 2. Standard Formats

### 2.1 Pagination Format

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number (1-indexed) |
| pageSize | integer | 20 | Items per page (min 1, max 100) |
| sortBy | string | createdAt | Sort field |
| sortOrder | string | desc | `asc` or `desc` |

**Response Envelope:**

```json
{
  "data": [],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

### 2.2 Error Response Format

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The request contains invalid fields",
    "details": [
      {
        "field": "email",
        "code": "INVALID_FORMAT",
        "message": "Must be a valid email address"
      }
    ],
    "requestId": "550e8400-e29b-41d4-a716-446655440000",
    "timestamp": "2026-07-28T10:30:00Z"
  }
}
```

**Error Codes:**

| HTTP Status | Code | When |
|-------------|------|------|
| 400 | VALIDATION_ERROR | Request body fails validation |
| 401 | UNAUTHORIZED | Missing or invalid token |
| 403 | FORBIDDEN | Insufficient permissions |
| 404 | NOT_FOUND | Resource does not exist |
| 409 | CONFLICT | Resource already exists (e.g., duplicate email) |
| 429 | RATE_LIMITED | Too many requests |
| 500 | INTERNAL_ERROR | Unexpected server error |
| 502 | BAD_GATEWAY | Upstream service (GLM API) error |
| 504 | GATEWAY_TIMEOUT | Upstream service timeout |

### 2.3 Success Response Envelope

Single resource:

```json
{
  "data": { }
}
```

Collection:

```json
{
  "data": [],
  "pagination": { }
}
```

---

## 3. Authentication

### 3.1 JWT Structure

**Header:**

```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload:**

```json
{
  "sub": "user-uuid",
  "email": "user@example.com",
  "role": "user",
  "iat": 1722168600,
  "exp": 1722169500
}
```

### 3.2 Token Lifecycle

```
Registration --> accessToken (15 min) + refreshToken (7 days)
Login         --> accessToken (15 min) + refreshToken (7 days)
Refresh       --> new accessToken (15 min) + new refreshToken (7 days, old invalidated)
Logout        --> refreshToken invalidated
Password Change --> all refreshTokens invalidated
```

### 3.3 Token Storage

- Access token: Memory (JavaScript variable) or HttpOnly cookie
- Refresh token: HttpOnly, Secure, SameSite=Strict cookie

---

## 4. Endpoint Reference

### 4.1 POST /auth/register

Register a new user.

**Auth Required:** No
**Rate Limit:** 5 requests/minute per IP

**Request Body:**

```json
{
  "email": "string (required, valid email format)",
  "password": "string (required, 8-128 chars, must include uppercase, lowercase, number)",
  "firstName": "string (required, 1-50 chars, letters and spaces only)",
  "lastName": "string (required, 1-50 chars, letters and spaces only)"
}
```

**Validation Rules:**
- email: Must be valid email format
- password: Min 8, max 128 chars, at least 1 uppercase, 1 lowercase, 1 number
- firstName: 1-50 chars, regex `^[a-zA-Z\s]+$`
- lastName: 1-50 chars, regex `^[a-zA-Z\s]+$`

**Response 201:**

```json
{
  "data": {
    "id": "uuid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "user",
    "createdAt": "2026-07-28T10:30:00Z"
  },
  "tokens": {
    "accessToken": "jwt-string",
    "refreshToken": "uuid-string",
    "expiresIn": 900
  }
}
```

**Error Responses:**
- 400: Validation error (details in error.details)
- 409: Email already exists

---

### 4.2 POST /auth/login

Authenticate an existing user.

**Auth Required:** No
**Rate Limit:** 10 requests/minute per IP

**Request Body:**

```json
{
  "email": "string (required)",
  "password": "string (required)"
}
```

**Response 200:**

```json
{
  "data": {
    "id": "uuid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "user"
  },
  "tokens": {
    "accessToken": "jwt-string",
    "refreshToken": "uuid-string",
    "expiresIn": 900
  }
}
```

**Error Responses:**
- 401: Invalid credentials
- 403: Account locked (too many failed attempts)

---

### 4.3 POST /auth/logout

Invalidate the current refresh token.

**Auth Required:** Yes
**Rate Limit:** 60 requests/minute

**Request Body:**

```json
{
  "refreshToken": "uuid-string (required)"
}
```

**Response 204:** No content

---

### 4.4 POST /auth/refresh-token

Get a new access token using a refresh token.

**Auth Required:** No (uses refresh token from body)
**Rate Limit:** 60 requests/minute

**Request Body:**

```json
{
  "refreshToken": "uuid-string (required)"
}
```

**Response 200:**

```json
{
  "tokens": {
    "accessToken": "jwt-string",
    "refreshToken": "uuid-string (new, old invalidated)",
    "expiresIn": 900
  }
}
```

**Error Responses:**
- 401: Invalid or expired refresh token

---

### 4.5 GET /auth/profile

Get the current user's profile.

**Auth Required:** Yes
**Rate Limit:** 60 requests/minute

**Response 200:**

```json
{
  "data": {
    "id": "uuid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "user",
    "profile": {
      "firstName": "John",
      "lastName": "Doe",
      "dateOfBirth": "1995-06-15",
      "gender": "male",
      "heightCm": 180,
      "weightKg": 82.5,
      "fitnessLevel": "intermediate",
      "goals": ["hypertrophy", "strength"],
      "equipmentAvailable": ["barbell", "dumbbells", "machines"],
      "limitations": "",
      "avatarUrl": null
    },
    "createdAt": "2026-07-28T10:30:00Z"
  }
}
```

---

### 4.6 PUT /auth/profile

Update the current user's profile.

**Auth Required:** Yes
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "firstName": "string (optional)",
  "lastName": "string (optional)",
  "dateOfBirth": "string (optional, ISO 8601 date)",
  "gender": "string (optional, enum)",
  "heightCm": "number (optional, 50-300)",
  "weightKg": "number (optional, 20-500)",
  "fitnessLevel": "string (optional, enum)",
  "goals": ["enum array (optional)"],
  "equipmentAvailable": ["string array (optional)"],
  "limitations": "string (optional, max 500 chars)",
  "avatarUrl": "string (optional)"
}
```

**Response 200:** Updated profile (same shape as GET /auth/profile)

---

### 4.7 POST /auth/change-password

Change the current user's password.

**Auth Required:** Yes
**Rate Limit:** 10 requests/minute

**Request Body:**

```json
{
  "currentPassword": "string (required)",
  "newPassword": "string (required, 8-128 chars, must include uppercase, lowercase, number)"
}
```

**Response 204:** No content

**Side Effects:** All refresh tokens for this user are invalidated.

**Error Responses:**
- 401: Current password is incorrect

---

### 4.8 GET /users

List all users (admin only).

**Auth Required:** Yes (admin)
**Rate Limit:** Unlimited

**Query Parameters:** page, pageSize, sortBy (name, email, createdAt), sortOrder

**Response 200:**

```json
{
  "data": [
    {
      "id": "uuid",
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": "user",
      "createdAt": "2026-07-28T10:30:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

---

### 4.9 GET /users/{id}

Get a user by ID. Users can only access their own profile. Admins can access any profile.

**Auth Required:** Yes (self or admin)
**Rate Limit:** 60 requests/minute

**Response 200:** User object (same as auth/profile response)

**Error Responses:**
- 403: Not authorized to view this user
- 404: User not found

---

### 4.10 GET /workout-plans

List the current user's workout plans.

**Auth Required:** Yes
**Rate Limit:** 60 requests/minute

**Query Parameters:**
- page, pageSize, sortBy (title, createdAt, status), sortOrder
- status: Filter by status (draft, active, completed, paused)
- planType: Filter by type (strength, hypertrophy, cardio, flexibility)

**Response 200:**

```json
{
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "planType": "hypertrophy",
      "title": "12-Week Hypertrophy Program",
      "description": "A comprehensive hypertrophy program...",
      "daysPerWeek": 4,
      "totalWeeks": 12,
      "status": "active",
      "generatedBy": "ai",
      "createdAt": "2026-07-28T10:30:00Z",
      "updatedAt": "2026-07-28T10:30:00Z"
    }
  ],
  "pagination": { }
}
```

---

### 4.11 GET /workout-plans/{id}

Get a workout plan with its sessions.

**Auth Required:** Yes (owner)
**Rate Limit:** 60 requests/minute

**Response 200:**

```json
{
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "planType": "hypertrophy",
    "title": "12-Week Hypertrophy Program",
    "description": "A comprehensive hypertrophy program...",
    "daysPerWeek": 4,
    "totalWeeks": 12,
    "status": "active",
    "generatedBy": "ai",
    "aiMetadata": {
      "modelVersion": "glm-4.7-flash",
      "promptVersion": "v1.2",
      "generationId": "uuid",
      "tokenUsage": {
        "promptTokens": 1200,
        "completionTokens": 3500,
        "totalTokens": 4700
      },
      "latencyMs": 12500,
      "generatedAt": "2026-07-28T10:30:00Z"
    },
    "sessions": [
      {
        "id": "uuid",
        "dayOfWeek": 1,
        "date": "2026-07-28T08:00:00Z",
        "title": "Day 1: Upper Body Push",
        "orderIndex": 0,
        "durationMinutes": null,
        "completed": false,
        "exerciseCount": 5,
        "completedSetCount": 0
      }
    ],
    "createdAt": "2026-07-28T10:30:00Z",
    "updatedAt": "2026-07-28T10:30:00Z"
  }
}
```

**Error Responses:**
- 403: Not authorized to view this plan
- 404: Plan not found

---

### 4.12 POST /workout-plans

Create a new workout plan.

**Auth Required:** Yes
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "title": "string (required, 1-255 chars)",
  "planType": "enum (required): strength | hypertrophy | cardio | flexibility",
  "description": "string (optional, max 2000 chars)",
  "daysPerWeek": "integer (required, 1-7)",
  "totalWeeks": "integer (required, 1-52)"
}
```

**Response 201:**

```json
{
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "planType": "hypertrophy",
    "title": "12-Week Hypertrophy Program",
    "description": "...",
    "daysPerWeek": 4,
    "totalWeeks": 12,
    "status": "draft",
    "generatedBy": "manual",
    "createdAt": "2026-07-28T10:30:00Z"
  }
}
```

**Error Responses:**
- 400: Validation error
- 403: Only one active plan allowed (if user already has active plan and trying to create active)

---

### 4.13 PUT /workout-plans/{id}

Update a workout plan.

**Auth Required:** Yes (owner)
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "title": "string (optional)",
  "description": "string (optional)",
  "planType": "enum (optional)",
  "daysPerWeek": "integer (optional, 1-7)",
  "totalWeeks": "integer (optional, 1-52)",
  "status": "enum (optional): draft | active | paused | completed"
}
```

**Status Transition Rules:**
- draft -> active
- active -> paused
- paused -> active
- active -> completed

**Response 200:** Updated workout plan

**Error Responses:**
- 400: Invalid status transition
- 403: Not authorized
- 404: Not found

---

### 4.14 DELETE /workout-plans/{id}

Soft-delete a workout plan.

**Auth Required:** Yes (owner)
**Rate Limit:** 10 requests/minute

**Response 204:** No content

**Side Effects:** Sets isDeleted = true, deletedAt = now. Plan is retained for 30 days.

---

### 4.15 POST /workout-plans/{id}/generate

Generate a workout plan using AI.

**Auth Required:** Yes (owner)
**Rate Limit:** 2 requests/hour (free), 10/hour (premium), unlimited (pro)

**Request Body:**

```json
{
  "fitnessLevel": "enum (required): beginner | intermediate | advanced",
  "goals": ["enum array (required): strength | hypertrophy | endurance | weight_loss | flexibility"],
  "equipment": ["string array (required): barbell | dumbbells | machines | bodyweight | bands"],
  "daysPerWeek": "integer (required, 1-7)",
  "totalWeeks": "integer (required, 1-52)",
  "focusAreas": ["string array (optional): chest | back | legs | shoulders | arms | core"],
  "limitations": "string (optional, max 500 chars)"
}
```

**Response 202:**

```json
{
  "data": {
    "generationId": "uuid",
    "status": "queued",
    "estimatedCompletionSeconds": 30,
    "websocketChannel": "ws://localhost:5000/ws/generations/uuid"
  }
}
```

**Error Responses:**
- 400: Validation error (e.g., missing fitnessLevel)
- 403: User profile incomplete (fitnessLevel or goals missing)
- 429: Rate limit exceeded

---

### 4.16 GET /workout-plans/{planId}/sessions

List sessions for a workout plan.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 60 requests/minute

**Query Parameters:** page, pageSize, sortBy (dayOfWeek, orderIndex), sortOrder

**Response 200:**

```json
{
  "data": [
    {
      "id": "uuid",
      "planId": "uuid",
      "dayOfWeek": 1,
      "date": "2026-07-28T08:00:00Z",
      "title": "Day 1: Upper Body Push",
      "description": "Chest, shoulders, and triceps",
      "orderIndex": 0,
      "durationMinutes": null,
      "caloriesBurned": null,
      "completed": false,
      "completedAt": null,
      "createdAt": "2026-07-28T10:30:00Z"
    }
  ],
  "pagination": { }
}
```

---

### 4.17 POST /workout-plans/{planId}/sessions

Add a session to a workout plan.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "dayOfWeek": "integer (required, 1-7)",
  "date": "string (required, ISO 8601)",
  "title": "string (required, 1-255 chars)",
  "description": "string (optional)",
  "orderIndex": "integer (required, 0-indexed position)"
}
```

**Response 201:** Created session object

---

### 4.18 GET /sessions/{id}

Get a session with its exercise sets.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 60 requests/minute

**Response 200:**

```json
{
  "data": {
    "id": "uuid",
    "planId": "uuid",
    "dayOfWeek": 1,
    "date": "2026-07-28T08:00:00Z",
    "title": "Day 1: Upper Body Push",
    "orderIndex": 0,
    "completed": false,
    "sets": [
      {
        "id": "uuid",
        "exerciseId": "uuid",
        "exerciseName": "Barbell Bench Press",
        "setNumber": 1,
        "reps": 10,
        "weightKg": 80.00,
        "completed": true,
        "notes": null,
        "createdAt": "2026-07-28T08:15:00Z"
      }
    ],
    "createdAt": "2026-07-28T10:30:00Z"
  }
}
```

---

### 4.19 PUT /sessions/{id}

Update a session.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "title": "string (optional)",
  "description": "string (optional)",
  "durationMinutes": "integer (optional)",
  "caloriesBurned": "integer (optional)"
}
```

**Response 200:** Updated session object

---

### 4.20 POST /sessions/{id}/complete

Mark a session as completed.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 30 requests/minute

**Response 200:**

```json
{
  "data": {
    "id": "uuid",
    "completed": true,
    "completedAt": "2026-07-28T09:30:00Z"
  }
}
```

**Error Responses:**
- 400: Not all sets are completed yet

---

### 4.21 GET /sessions/{sessionId}/sets

List sets for a session.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 60 requests/minute

**Response 200:**

```json
{
  "data": [
    {
      "id": "uuid",
      "workoutId": "uuid",
      "exerciseId": "uuid",
      "exerciseName": "Barbell Bench Press",
      "setNumber": 1,
      "reps": 10,
      "weightKg": 80.00,
      "completed": true,
      "notes": null,
      "createdAt": "2026-07-28T08:15:00Z"
    }
  ]
}
```

---

### 4.22 POST /sessions/{sessionId}/sets

Log a set for a session.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "exerciseId": "uuid (required)",
  "setNumber": "integer (required, >= 1)",
  "reps": "integer (optional, 0-99)",
  "weightKg": "number (optional, 0-999)",
  "notes": "string (optional, max 500 chars)"
}
```

**Response 201:**

```json
{
  "data": {
    "id": "uuid",
    "workoutId": "uuid",
    "exerciseId": "uuid",
    "setNumber": 1,
    "reps": 10,
    "weightKg": 80.00,
    "completed": false,
    "notes": null,
    "createdAt": "2026-07-28T08:15:00Z"
  }
}
```

**Error Responses:**
- 400: Validation error (reps > 99, weightKg > 999, etc.)
- 404: Exercise not found in catalog

---

### 4.23 PUT /sets/{id}

Update a set.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "reps": "integer (optional, 0-99)",
  "weightKg": "number (optional, 0-999)",
  "completed": "boolean (optional)",
  "notes": "string (optional)"
}
```

**Response 200:** Updated set object

**Error Responses:**
- 403: Set cannot be edited after 24 hours

---

### 4.24 DELETE /sets/{id}

Delete a set.

**Auth Required:** Yes (plan owner)
**Rate Limit:** 30 requests/minute

**Response 204:** No content

**Error Responses:**
- 403: Set cannot be deleted after 24 hours

---

### 4.25 GET /exercises

List exercises from the catalog.

**Auth Required:** No (public catalog)
**Rate Limit:** 60 requests/minute per IP

**Query Parameters:**
- page, pageSize, sortBy (name, category, difficulty), sortOrder
- category: Filter by category (chest, back, legs, shoulders, arms, core, cardio)
- difficulty: Filter by difficulty (beginner, intermediate, advanced)
- equipment: Filter by equipment name
- search: Search by exercise name

**Response 200:**

```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Barbell Bench Press",
      "category": "chest",
      "difficulty": "intermediate",
      "equipment": "Barbell, Bench",
      "instructions": "Lie on bench, grip barbell slightly wider than shoulder width...",
      "muscleGroup": ["chest", "triceps", "front_delts"],
      "createdAt": "2026-07-28T10:30:00Z"
    }
  ],
  "pagination": { }
}
```

---

### 4.26 GET /exercises/{id}

Get an exercise by ID.

**Auth Required:** No
**Rate Limit:** 60 requests/minute per IP

**Response 200:** Exercise object

**Error Responses:**
- 404: Exercise not found

---

### 4.27 POST /exercises

Create a new exercise (admin only).

**Auth Required:** Yes (admin)
**Rate Limit:** Unlimited

**Request Body:**

```json
{
  "name": "string (required, 1-100 chars, unique)",
  "category": "enum (required): chest | back | legs | shoulders | arms | core | cardio",
  "difficulty": "enum (required): beginner | intermediate | advanced",
  "equipment": "string (optional, max 100 chars)",
  "instructions": "string (optional)",
  "muscleGroup": ["string array (optional)"]
}
```

**Response 201:** Created exercise object

**Error Responses:**
- 409: Exercise name already exists

---

### 4.28 PUT /exercises/{id}

Update an exercise (admin only).

**Auth Required:** Yes (admin)
**Rate Limit:** Unlimited

**Request Body:** Same as POST, all fields optional

**Response 200:** Updated exercise object

---

### 4.29 DELETE /exercises/{id}

Delete an exercise (admin only).

**Auth Required:** Yes (admin)
**Rate Limit:** Unlimited

**Response 204:** No content

**Error Responses:**
- 409: Exercise is referenced by existing ExerciseSets

---

### 4.30 GET /exercises/categories

List all exercise categories.

**Auth Required:** No
**Rate Limit:** 60 requests/minute per IP

**Response 200:**

```json
{
  "data": [
    { "name": "chest", "displayName": "Chest", "exerciseCount": 12 },
    { "name": "back", "displayName": "Back", "exerciseCount": 15 },
    { "name": "legs", "displayName": "Legs", "exerciseCount": 18 },
    { "name": "shoulders", "displayName": "Shoulders", "exerciseCount": 8 },
    { "name": "arms", "displayName": "Arms", "exerciseCount": 10 },
    { "name": "core", "displayName": "Core", "exerciseCount": 9 },
    { "name": "cardio", "displayName": "Cardio", "exerciseCount": 6 }
  ]
}
```

---

### 4.31 GET /progress

Get the current user's progress analytics.

**Auth Required:** Yes
**Rate Limit:** 60 requests/minute

**Query Parameters:**
- period: `week`, `month`, `3months`, `6months`, `year` (default: `month`)
- exerciseId: Filter by specific exercise

**Response 200:**

```json
{
  "data": {
    "summary": {
      "totalWorkouts": 24,
      "totalVolume": 125000.50,
      "personalRecords": 8,
      "currentStreak": 5,
      "longestStreak": 12
    },
    "volumeByWeek": [
      { "week": "2026-W28", "volume": 12500.00 },
      { "week": "2026-W29", "volume": 13200.50 }
    ],
    "weightProgression": [
      { "date": "2026-07-01", "weightKg": 80.0 },
      { "date": "2026-07-15", "weightKg": 82.5 }
    ],
    "recentPRs": [
      {
        "exerciseName": "Barbell Bench Press",
        "weightKg": 100.0,
        "reps": 8,
        "achievedAt": "2026-07-28T08:30:00Z"
      }
    ]
  }
}
```

---

### 4.32 GET /progress/history

Get the current user's progress log history.

**Auth Required:** Yes
**Rate Limit:** 60 requests/minute

**Query Parameters:** page, pageSize, sortBy (measurementDate), sortOrder, exerciseId

**Response 200:**

```json
{
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "workoutId": "uuid",
      "exerciseId": "uuid",
      "exerciseName": "Barbell Bench Press",
      "measurementDate": "2026-07-28T08:30:00Z",
      "weightKg": 82.5,
      "reps": 10,
      "sets": 3,
      "notes": "Felt strong today",
      "createdAt": "2026-07-28T08:30:00Z"
    }
  ],
  "pagination": { }
}
```

---

### 4.33 POST /progress/log

Log a progress entry.

**Auth Required:** Yes
**Rate Limit:** 30 requests/minute

**Request Body:**

```json
{
  "workoutId": "uuid (optional)",
  "exerciseId": "uuid (optional)",
  "measurementDate": "string (optional, ISO 8601, default: now)",
  "weightKg": "number (optional, 20-500)",
  "reps": "integer (optional, 0-99)",
  "sets": "integer (optional, 1-20)",
  "notes": "string (optional, max 500 chars)"
}
```

**Response 201:**

```json
{
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "workoutId": "uuid",
    "exerciseId": "uuid",
    "measurementDate": "2026-07-28T08:30:00Z",
    "weightKg": 82.5,
    "reps": 10,
    "sets": 3,
    "notes": "Felt strong today",
    "createdAt": "2026-07-28T08:30:00Z"
  }
}
```

---

### 4.34 GET /subscriptions

Get the current user's subscription.

**Auth Required:** Yes
**Rate Limit:** 30 requests/minute

**Response 200:**

```json
{
  "data": {
    "id": "uuid",
    "userId": "uuid",
    "tier": "free",
    "status": "active",
    "startedAt": "2026-07-28T10:30:00Z",
    "expiresAt": null,
    "paymentProvider": null,
    "createdAt": "2026-07-28T10:30:00Z"
  }
}
```

---

### 4.35 POST /subscriptions/upgrade

Upgrade the current user's subscription tier.

**Auth Required:** Yes
**Rate Limit:** 5 requests/minute

**Request Body:**

```json
{
  "tier": "enum (required): premium | pro"
}
```

**Response 200:** Updated subscription object

**Note:** In MVP, this is a manual toggle (no Stripe integration). In production, this would redirect to a payment flow.

---

### 4.36 POST /subscriptions/cancel

Cancel the current user's subscription.

**Auth Required:** Yes
**Rate Limit:** 5 requests/minute

**Response 200:** Updated subscription object with status = 'cancelled'

---

## 5. Rate Limiting Summary

| Endpoint | Unauthenticated | Free | Premium | Pro | Admin |
|----------|----------------|------|---------|-----|-------|
| POST /auth/register | 5/min/IP | - | - | - | - |
| POST /auth/login | 10/min/IP | - | - | - | - |
| POST /auth/refresh-token | - | 60/min | 60/min | 60/min | 60/min |
| GET /auth/profile | - | 60/min | 60/min | 60/min | 60/min |
| PUT /auth/profile | - | 30/min | 30/min | 30/min | 30/min |
| GET /workout-plans | - | 60/min | 60/min | 60/min | 60/min |
| POST /workout-plans | - | 30/min | 30/min | 30/min | 30/min |
| POST /workout-plans/{id}/generate | - | 2/hour | 10/hour | Unlimited | Unlimited |
| GET /exercises | 60/min/IP | 60/min | 60/min | 60/min | Unlimited |
| POST /exercises | - | - | - | - | Unlimited |
| GET /progress | - | 60/min | 60/min | 60/min | 60/min |
| POST /progress/log | - | 30/min | 30/min | 30/min | 30/min |

---

## 6. WebSocket Events

### 6.1 Generation Progress

**Channel:** `ws://api/ws/generations/{generationId}`

**Events:**

```json
{
  "event": "generation.progress",
  "data": {
    "generationId": "uuid",
    "status": "processing",
    "progressPercent": 45,
    "message": "Generating week 3 of 12..."
  }
}
```

```json
{
  "event": "generation.completed",
  "data": {
    "generationId": "uuid",
    "status": "completed",
    "planId": "uuid"
  }
}
```

```json
{
  "event": "generation.failed",
  "data": {
    "generationId": "uuid",
    "status": "failed",
    "error": "AI service temporarily unavailable",
    "fallbackUsed": true
  }
}
```
