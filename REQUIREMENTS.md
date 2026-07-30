# FitForge AI - Requirements Specification

**Purpose:** Formal requirements that AI coding agents and developers use to verify implementation completeness.

**Version:** 1.0
**Last Updated:** 2026-07-28

**Depends on:** DOMAIN_GLOSSARY.md (all entity names and terms are defined there)

---

## 1. Functional Requirements

### 1.1 FR-AUTH: Authentication & Authorization

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-AUTH-001 | System shall register users with email and password | P0 |
| FR-AUTH-002 | System shall hash passwords using bcrypt with cost factor 12 | P0 |
| FR-AUTH-003 | System shall issue JWT access tokens with 15-minute expiry | P0 |
| FR-AUTH-004 | System shall issue refresh tokens with 7-day expiry | P0 |
| FR-AUTH-005 | System shall rotate refresh tokens on use (old token invalidated) | P1 |
| FR-AUTH-006 | System shall support role-based access: user, admin, premium | P1 |
| FR-AUTH-007 | System shall send email verification on registration | P1 |
| FR-AUTH-008 | System shall support password reset via email | P2 |
| FR-AUTH-009 | System shall invalidate all refresh tokens on password change | P1 |
| FR-AUTH-010 | System shall lock account after 5 failed login attempts (15-minute lockout) | P2 |

---

### 1.2 FR-WORKOUT: Workout Management

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-WORKOUT-001 | Users shall create workout plans with title, planType, daysPerWeek, totalWeeks | P0 |
| FR-WORKOUT-002 | Users shall add WorkoutSessions to a WorkoutPlan with dayOfWeek and orderIndex | P0 |
| FR-WORKOUT-003 | Users shall add ExerciseSets to a WorkoutSession with exerciseId, setNumber, reps, weightKg | P0 |
| FR-WORKOUT-004 | Users shall mark ExerciseSets as completed | P0 |
| FR-WORKOUT-005 | System shall auto-complete a WorkoutSession when all its ExerciseSets are completed | P0 |
| FR-WORKOUT-006 | Users shall mark a WorkoutPlan as active, paused, or completed | P0 |
| FR-WORKOUT-007 | Users shall delete their own WorkoutPlans (soft delete) | P1 |
| FR-WORKOUT-008 | Users shall filter WorkoutPlans by status, planType, and date | P1 |
| FR-WORKOUT-009 | Users shall sort WorkoutPlans by createdAt, title, and status | P1 |
| FR-WORKOUT-010 | System shall paginate WorkoutPlan lists (default 20 per page, max 100) | P1 |

---

### 1.3 FR-AI: AI Workout Generation

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-AI-001 | System shall generate WorkoutPlans from user preferences via GLM 4.7 Flash API | P0 |
| FR-AI-002 | Generation shall be asynchronous (queue-based) with progress notification via WebSocket | P0 |
| FR-AI-003 | System shall validate AI-generated plans against exercise catalog (name matching) | P0 |
| FR-AI-004 | System shall fall back to template workouts if AI API fails after 3 retries | P0 |
| FR-AI-005 | System shall cache generated plans by preference hash (SHA256) with 7-day TTL | P1 |
| FR-AI-006 | System shall store AI generation metadata (model version, tokens, latency) | P1 |
| FR-AI-007 | System shall validate user has completed profile before allowing AI generation | P1 |
| FR-AI-008 | System shall rate-limit AI generation to 2 requests/hour for free users | P1 |

---

### 1.4 FR-PROGRESS: Progress Tracking

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-PROGRESS-001 | Users shall log body weight and notes as ProgressLog entries | P0 |
| FR-PROGRESS-002 | System shall calculate personal records (PRs) per Exercise | P1 |
| FR-PROGRESS-003 | System shall display progress charts: volume over time, weight progression | P1 |
| FR-PROGRESS-004 | System shall compute total volume (sets x reps x weight) per session | P1 |
| FR-PROGRESS-005 | ProgressLogs shall be append-only (no updates, no deletes) | P1 |

---

### 1.5 FR-EXERCISE: Exercise Catalog

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-EXERCISE-001 | System shall provide a read-only exercise catalog for authenticated users | P0 |
| FR-EXERCISE-002 | Admins shall create, update, and delete exercises | P0 |
| FR-EXERCISE-003 | Users shall filter exercises by category, difficulty, and equipment | P1 |
| FR-EXERCISE-004 | System shall prevent exercise deletion if referenced by any ExerciseSet | P1 |

---

### 1.6 FR-SUBSCRIPTION: Subscription Management

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-SUBSCRIPTION-001 | System shall assign a free-tier subscription to every new User | P0 |
| FR-SUBSCRIPTION-002 | System shall check subscription tier before allowing premium features | P1 |
| FR-SUBSCRIPTION-003 | Users shall upgrade their subscription tier | P2 |

---

## 2. Non-Functional Requirements

### 2.1 Performance

| ID | Requirement | Target |
|----|-------------|--------|
| NFR-PERF-001 | API response time (P95) for non-AI endpoints | < 500ms |
| NFR-PERF-002 | AI generation time (P95) | < 30 seconds |
| NFR-PERF-003 | Time to first byte (TTFB) | < 200ms |
| NFR-PERF-004 | Database query time for indexed queries (P95) | < 50ms |

### 2.2 Scalability

| ID | Requirement | Target |
|----|-------------|--------|
| NFR-SCALE-001 | Concurrent authenticated users | 10,000 |
| NFR-SCALE-002 | Database connection pool size | 100 connections |

### 2.3 Availability

| ID | Requirement | Target |
|----|-------------|--------|
| NFR-AVAIL-001 | System uptime | 99.9% |
| NFR-AVAIL-002 | Recovery time objective (RTO) | < 1 hour |
| NFR-AVAIL-003 | Recovery point objective (RPO) | < 5 minutes |

### 2.4 Security

| ID | Requirement | Target |
|----|-------------|--------|
| NFR-SEC-001 | Password hashing algorithm | bcrypt, cost factor 12 |
| NFR-SEC-002 | JWT signing algorithm | HS256 |
| NFR-SEC-003 | HTTPS enforcement | All endpoints, no exceptions |
| NFR-SEC-004 | Rate limiting | Per-endpoint, per-role (see API_SPEC.md) |

### 2.5 Observability

| ID | Requirement | Target |
|----|-------------|--------|
| NFR-OBS-001 | Structured logging format | Serilog, JSON output |
| NFR-OBS-002 | Distributed tracing | OpenTelemetry |
| NFR-OBS-003 | Error tracking | Sentry integration |

---

## 3. Business Rules

| ID | Rule | Domain | Enforced At |
|----|------|--------|-------------|
| BR-001 | A User can have multiple WorkoutPlans, but only one can be `active` at a time | Workout | Backend validation |
| BR-002 | AI generation requires a completed UserProfile (fitnessLevel and goals must be set) | AI | Backend validation |
| BR-003 | ExerciseSet reps must be 0-99 (or null for timed exercises) | Workout | Backend + Frontend validation |
| BR-004 | ExerciseSet weightKg must be 0-999 (or null for bodyweight) | Workout | Backend + Frontend validation |
| BR-005 | A WorkoutSession auto-completes when all its ExerciseSets are completed | Workout | Backend event handler |
| BR-006 | AI generation is rate-limited to 2 requests/hour for free users, 10 for premium, unlimited for pro | AI | Backend rate limiter |
| BR-007 | Deleted WorkoutPlans are soft-deleted (retained 30 days, then purged) | Workout | Backend + Scheduled job |
| BR-008 | Exercise catalog entries can only be created/modified by admin users | Exercise | Backend authorization |
| BR-009 | ExerciseSets cannot be edited after 24 hours from creation | Workout | Backend validation |
| BR-010 | Refresh tokens are invalidated when user changes password | Auth | Backend event handler |
| BR-011 | A Subscription record is created automatically on User registration (free tier) | Subscription | Backend event handler |
| BR-012 | WorkoutPlan status transitions are restricted: draft->active, active->paused, paused->active, active->completed | Workout | Backend validation |
| BR-013 | AI-generated WorkoutPlans must have all exercises matched to the catalog (fuzzy match allowed) | AI | Backend validation |
| BR-014 | ProgressLog entries are append-only (no updates, no deletes) | Progress | Backend enforcement |
| BR-015 | Users can only view/modify their own data (WorkoutPlans, ProgressLogs, Subscription) | All | Backend authorization |

---

## 4. Constraints

| ID | Constraint | Reason |
|----|------------|--------|
| C-001 | Must use ASP.NET Core 8.0 for backend | Team expertise, existing investment |
| C-002 | Must use React 18 + TypeScript for frontend | Team expertise, ecosystem |
| C-003 | Must use PostgreSQL 15+ as primary database | JSONB support, extensions, cost |
| C-004 | Must use GLM 4.7 Flash as AI provider | Budget, performance requirements |
| C-005 | Must run in Docker for local development | Environment consistency |
| C-006 | Must deploy to AWS | Existing infrastructure |
| C-007 | Must support modern browsers: Chrome, Firefox, Safari, Edge (last 2 versions) | User base |
| C-008 | AWS costs must stay under $1,200/month at 10k users | Financial |

---

## 5. Assumptions

| ID | Assumption | Risk if Wrong | Mitigation |
|----|------------|---------------|------------|
| A-001 | GLM 4.7 Flash API is available and stable | High - core feature depends on it | Fallback to template workouts |
| A-002 | Users have reliable internet for AI generation | Medium - generation may fail | Async processing with retry |
| A-003 | Exercise catalog is static (admin-managed) | Low - can add user submissions later | Schema supports future extension |
| A-004 | Single-language (English) for MVP | Low - i18n deferred | All strings externalized |
| A-005 | No payment processing in MVP | Low - subscription is feature-flagged | Stripe integration deferred |
| A-006 | Team has ASP.NET Core expertise | Medium - onboarding needed if wrong | Documentation + pair programming |

---

## 6. Acceptance Criteria

### AC-AUTH-001: User Registration

```
Given a visitor on the registration page
When they provide a valid email, password, firstName, and lastName
And submit the registration form
Then a new User record is created with role = 'user'
And the password is stored as a bcrypt hash (cost 12)
And a free-tier Subscription is created for the User
And a verification email is sent to the provided email
And the response contains an accessToken (15 min expiry) and refreshToken (7 day expiry)
```

```
Given a visitor on the registration page
When they provide an email that already exists
And submit the registration form
Then a 409 Conflict error is returned
And no User record is created
```

### AC-AUTH-002: User Login

```
Given a registered and verified user
When they provide correct email and password
And submit the login form
Then a 200 OK response is returned
And the response contains an accessToken (15 min expiry) and refreshToken (7 day expiry)
```

```
Given a registered user
When they provide incorrect password
And submit the login form
Then a 401 Unauthorized error is returned
And the failed attempt is recorded
And after 5 failed attempts, the account is locked for 15 minutes
```

### AC-WORKOUT-001: Manual Workout Plan Creation

```
Given an authenticated user
When they create a WorkoutPlan with title, planType, daysPerWeek=4, totalWeeks=12
Then a new WorkoutPlan is created with status = 'draft'
And the plan has no WorkoutSessions initially
And the plan is returned in the response
```

```
Given an authenticated user with an active WorkoutPlan
When they attempt to create another WorkoutPlan with status = 'active'
Then a 400 Bad Request error is returned
With message: "Only one active workout plan allowed at a time"
```

### AC-WORKOUT-002: Workout Session Logging

```
Given an authenticated user with an active WorkoutPlan
When they log an ExerciseSet with exerciseId, setNumber=1, reps=10, weightKg=80
Then a new ExerciseSet is created with completed = false
And the WorkoutSession status changes to 'in_progress'
```

```
Given a WorkoutSession with 3 ExerciseSets
When the user marks the last ExerciseSet as completed
Then the WorkoutSession auto-completes (completed = true, completedAt = now)
```

### AC-AI-001: AI Workout Generation

```
Given an authenticated user with a completed UserProfile
When they request AI generation with fitnessLevel='intermediate', goal='hypertrophy',
     equipment=['barbell','dumbbells'], daysPerWeek=4, totalWeeks=12
Then a 202 Accepted response is returned with generationId and websocketChannel
And a GenerateWorkoutCommand is enqueued to RabbitMQ
```

```
Given a queued GenerateWorkoutCommand
When the worker processes it
Then the GLM API is called with the user preferences
And the response is validated against the AI output schema
And exercises are matched to the catalog (exact, fuzzy, or fallback)
And a new WorkoutPlan is created with generatedBy = 'ai'
And aiMetadata is populated with model version, tokens, latency
And the user is notified via WebSocket
```

```
Given a queued GenerateWorkoutCommand
When the GLM API fails after 3 retries
Then a template WorkoutPlan is created (same daysPerWeek, totalWeeks)
And the user is notified that a fallback plan was used
And the failure is logged for monitoring
```

### AC-PROGRESS-001: Progress Logging

```
Given an authenticated user
When they log a ProgressLog with weightKg=85.5 and notes="Feeling strong"
Then a new ProgressLog is created with measurementDate = now
And the ProgressLog is returned in the response
```

```
Given a ProgressLog that was created more than 24 hours ago
When any user attempts to update or delete it
Then a 403 Forbidden error is returned
With message: "Progress logs cannot be modified after 24 hours"
```

---

## 7. Definition of Done

### Feature-Level Definition of Done

A feature is considered complete when:

- [ ] All P0 requirements for the feature are implemented
- [ ] All P1 requirements for the feature are implemented
- [ ] Unit tests pass with >= 80% coverage for service layer
- [ ] Integration tests pass for all API endpoints
- [ ] API response matches the contract in API_SPEC.md
- [ ] Frontend handles all states: loading, success, error, empty
- [ ] Validation rules match REQUIREMENTS.md
- [ ] No security vulnerabilities (input validation, auth checks)
- [ ] Code follows CODING_STANDARDS.md (naming, error handling, patterns)
- [ ] Code review completed

### Sprint-Level Definition of Done

A sprint is considered complete when:

- [ ] All features in the sprint meet feature-level DoD
- [ ] All integration tests pass
- [ ] All unit tests pass
- [ ] No regressions in existing features
- [ ] API documentation is updated
- [ ] Domain glossary is updated (if new terms introduced)
- [ ] Deployed to staging environment

### Release-Level Definition of Done

A release is considered complete when:

- [ ] All MVP features meet feature-level DoD
- [ ] All integration tests pass
- [ ] All E2E tests pass for critical paths
- [ ] No critical or high-severity security vulnerabilities
- [ ] Performance targets met (NFR-PERF-001 through NFR-PERF-004)
- [ ] Deployed to staging and smoke tested
- [ ] Rollback plan documented
- [ ] Monitoring and alerting configured
