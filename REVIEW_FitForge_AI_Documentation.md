# FitForge AI - Documentation Review

**Reviewed By:** Principal Software Architect
**Date:** 2026-07-28
**Documents Reviewed:** doc.txt (Project Milestones), doc2.txt (Architecture Design)
**Purpose:** Evaluate production-readiness for AI coding agent consumption

---

## EXECUTIVE SUMMARY

The FitForge AI documentation contains two files: a **Project Milestones** document (task-level breakdown) and an **Architecture Design** document (system-level design). While both documents demonstrate significant effort, they are **not production-ready** for AI coding agent consumption.

The architecture document is the stronger of the two, but suffers from critical gaps in domain modeling, API specification, and frontend readiness. The milestones document conflates project management tasks with technical specifications and lacks the requirements foundation needed before task breakdown can occur.

**Overall Score: 4.5 / 10**

---

## CRITERION 1: PRODUCT PLANNING

### Issues Found

#### Issue 1.1: No Vision Statement

**Why It Matters:** Without a vision statement, an AI coding agent cannot make architectural decisions that align with product direction. Every feature decision requires understanding what the product ultimately aspires to be.

**Recommendation:** Add a concise vision statement at the top of the architecture document.

**Proposed Revision:**

```markdown
## 1.1 Product Vision

FitForge AI is an AI-powered fitness platform that generates personalized workout
plans, tracks user progress, and adapts recommendations over time. The platform
targets fitness enthusiasts and beginners who want professional-grade workout
programming without hiring a personal trainer.

Primary Value Proposition: "Your AI personal trainer that learns from your
progress and adapts your program in real time."
```

---

#### Issue 1.2: No Business Goals Definition

**Why It Matters:** Business goals determine feature prioritization. An AI coding agent building a subscription system needs to know whether the goal is revenue generation, user acquisition, or retention.

**Recommendation:** Define explicit business goals.

**Proposed Revision:**

```markdown
## 1.2 Business Goals

| Goal | Target | Timeline |
|------|--------|----------|
| User Acquisition | 10,000 registered users | Month 6 |
| User Retention | 60% DAU/MAU ratio | Month 6 |
| Revenue | Premium subscription conversion > 5% | Month 6 |
| Platform Reliability | 99.9% uptime | Ongoing |
| Cost Efficiency | AWS bill < $1,200/month at 10k users | Month 6 |
```

---

#### Issue 1.3: No Explicit In-Scope / Out-of-Scope Definition

**Why It Matters:** An AI coding agent will build whatever is documented. Without explicit scope boundaries, it may build features that are out of scope (e.g., mobile app, social features) or miss features that are in scope.

**Recommendation:** Add explicit scope sections.

**Proposed Revision:**

```markdown
## 1.3 Project Scope

### In-Scope (MVP - Months 1-3)
- User registration, login, and profile management
- Manual workout plan creation and editing
- Exercise catalog with categories and filtering
- Workout session logging (sets, reps, weight)
- Basic progress tracking (PRs, volume over time)
- AI-powered workout plan generation (GLM 4.7 Flash)
- JWT authentication with refresh tokens
- PostgreSQL persistence
- Redis caching
- Responsive web application (desktop + mobile browser)
- Docker-based local development

### Out-of-Scope (Future Phases)
- Native mobile applications (iOS/Android)
- Social features (sharing, leaderboards, challenges)
- Nutrition tracking or calorie counting
- Wearable device integration (Apple Watch, Fitbit)
- Video exercise demonstrations
- Payment processing (Stripe integration deferred)
- Multi-language / internationalization
- Offline mode
- Voice-based interactions
```

---

#### Issue 1.4: No MVP Definition

**Why It Matters:** The migration strategy section (16.1) partially defines MVP scope but mixes it with technical decisions. The milestones document breaks MVP into 10 milestones without clearly stating which milestones constitute the MVP.

**Recommendation:** Create a clear MVP definition that maps to milestones.

**Proposed Revision:**

```markdown
## 1.4 MVP Definition

The MVP includes Milestones 1-7 and a subset of Milestone 8.

### MVP Feature Set
1. User registration and authentication (Milestone 2)
2. Exercise catalog with CRUD (Milestone 3)
3. Manual workout plan creation (Milestone 5)
4. Workout session logging (Milestone 5)
5. AI-powered workout generation (Milestone 6)
6. Progress tracking with charts (Milestone 7)
7. Basic analytics dashboard (Milestone 7)

### MVP Quality Gates
- All unit tests passing
- Integration tests for critical paths (auth, workout CRUD, AI generation)
- Deployed to staging environment
- No critical or high-severity security vulnerabilities

### Deferred to Post-MVP
- Kubernetes deployment (Milestone 9)
- Multi-region support
- Advanced monitoring (Sentry, Grafana)
- Admin dashboard
- Subscription management with payment
```

---

#### Issue 1.5: No Success Metrics Tied to Milestones

**Why It Matters:** Success metrics are defined in section 18 of the architecture document but not linked to milestones. An AI coding agent building a milestone needs to know what "done" means in measurable terms.

**Recommendation:** Add success criteria to each milestone.

---

## CRITERION 2: REQUIREMENTS ENGINEERING

### Issues Found

#### Issue 2.1: No Functional Requirements (FR) Document

**Why It Matters:** The architecture document describes system behavior informally but does not define formal functional requirements. An AI coding agent needs explicit FRs to know exactly what the system must do.

**Recommendation:** Create a formal FR section.

**Proposed Revision:**

```markdown
## 2.1 Functional Requirements

### FR-AUTH: Authentication & Authorization

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-AUTH-001 | System shall register users with email and password | P0 |
| FR-AUTH-002 | System shall hash passwords using bcrypt (cost factor 12) | P0 |
| FR-AUTH-003 | System shall issue JWT access tokens (15-minute expiry) | P0 |
| FR-AUTH-004 | System shall issue refresh tokens (7-day expiry) | P0 |
| FR-AUTH-005 | System shall rotate refresh tokens on use | P1 |
| FR-AUTH-006 | System shall support role-based access (user, admin, premium) | P1 |
| FR-AUTH-007 | System shall send email verification on registration | P1 |
| FR-AUTH-008 | System shall support password reset via email | P2 |

### FR-WORKOUT: Workout Management

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-WORKOUT-001 | Users shall create workout plans with title, type, days/week, total weeks | P0 |
| FR-WORKOUT-002 | Users shall add exercises to workout sessions from the catalog | P0 |
| FR-WORKOUT-003 | Users shall log sets with reps, weight, and completion status | P0 |
| FR-WORKOUT-004 | Users shall mark workout sessions as completed | P0 |
| FR-WORKOUT-005 | System shall track workout history per plan | P1 |
| FR-WORKOUT-006 | Users shall filter and sort workout plans by status, type, date | P1 |

### FR-AI: AI Workout Generation

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-AI-001 | System shall generate workout plans from user preferences via GLM API | P0 |
| FR-AI-002 | Generation shall be async (queue-based) with progress notification | P0 |
| FR-AI-003 | System shall validate AI-generated plans against exercise catalog | P0 |
| FR-AI-004 | System shall fall back to template workouts if AI fails | P1 |
| FR-AI-005 | System shall cache generated plans by preference hash (TTL 7 days) | P1 |

### FR-PROGRESS: Progress Tracking

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-PROGRESS-001 | Users shall log body weight and notes per workout session | P0 |
| FR-PROGRESS-002 | System shall calculate personal records per exercise | P1 |
| FR-PROGRESS-003 | System shall display progress charts (volume, weight over time) | P1 |
| FR-PROGRESS-004 | System shall compute strength gains per muscle group | P2 |
```

---

#### Issue 2.2: No Non-Functional Requirements (NFR)

**Why It Matters:** The architecture document has performance numbers (section 13) and success metrics (section 18) but these are not formal NFRs. An AI coding agent needs explicit NFRs to make technology and design choices.

**Recommendation:** Add formal NFRs.

**Proposed Revision:**

```markdown
## 2.2 Non-Functional Requirements

| ID | Category | Requirement | Target |
|----|----------|-------------|--------|
| NFR-PERF-001 | Performance | API response time (P95) | < 500ms |
| NFR-PERF-002 | Performance | AI generation time (P95) | < 30s |
| NFR-PERF-003 | Performance | Time to first byte (TTFB) | < 200ms |
| NFR-SCALE-001 | Scalability | Concurrent users | 10,000 |
| NFR-SCALE-002 | Scalability | Database connections | 100 pooled |
| NFR-AVAIL-001 | Availability | Uptime | 99.9% |
| NFR-AVAIL-002 | Availability | Recovery time objective (RTO) | < 1 hour |
| NFR-AVAIL-003 | Availability | Recovery point objective (RPO) | < 5 minutes |
| NFR-SEC-001 | Security | Password hashing | bcrypt, cost 12 |
| NFR-SEC-002 | Security | Token algorithm | HS256 |
| NFR-SEC-003 | Security | HTTPS enforcement | All endpoints |
| NFR-SEC-004 | Security | Rate limiting | Per-role limits (see 5.2) |
| NFR-OBS-001 | Observability | Structured logging | Serilog, JSON format |
| NFR-OBS-002 | Observability | Distributed tracing | OpenTelemetry |
| NFR-OBS-003 | Observability | Error tracking | Sentry |
```

---

#### Issue 2.3: No Business Rules

**Why It Matters:** Business rules constrain system behavior. Without them, an AI coding agent makes assumptions about edge cases.

**Recommendation:** Define business rules explicitly.

**Proposed Revision:**

```markdown
## 2.3 Business Rules

| ID | Rule | Scope |
|----|------|-------|
| BR-001 | A user can have multiple workout plans, but only one can be "active" at a time | Workout |
| BR-002 | Workout plans can only be generated for users with completed profiles (height, weight, level) | AI |
| BR-003 | Exercise sets cannot exceed 99 reps or 999 kg | Workout |
| BR-004 | A workout session can only be marked complete if all sets are logged | Workout |
| BR-005 | Premium features require active subscription | Auth |
| BR-006 | AI generation is rate-limited to 2 requests/hour for free users | AI |
| BR-007 | Deleted workout plans are soft-deleted (retained 30 days) | Workout |
| BR-008 | Exercise catalog entries can only be created/modified by admins | Exercise |
| BR-009 | Progress logs cannot be edited after 24 hours | Progress |
| BR-010 | Refresh tokens are invalidated on password change | Auth |
```

---

#### Issue 2.4: No Constraints Defined

**Why It Matters:** Constraints shape architectural decisions. Without them, an AI coding agent may choose technologies or patterns that violate project constraints.

**Recommendation:** Add constraints section.

**Proposed Revision:**

```markdown
## 2.4 Constraints

| ID | Constraint | Reason |
|----|------------|--------|
| C-001 | Must use ASP.NET Core 8.0 for backend | Team expertise, existing investment |
| C-002 | Must use React 18 + TypeScript for frontend | Team expertise, ecosystem |
| C-003 | Must use PostgreSQL as primary database | JSONB support, cost |
| C-004 | Must use GLM 4.7 Flash as AI provider | Budget, performance requirements |
| C-005 | Must run in Docker for local development | Environment consistency |
| C-006 | Must deploy to AWS | Existing infrastructure |
| C-007 | Must support modern browsers (Chrome, Firefox, Safari, Edge - last 2 versions) | User base |
| C-008 | Budget: < $1,200/month AWS costs at 10k users | Financial |
```

---

#### Issue 2.5: No Assumptions Defined

**Why It Matters:** Assumptions can be wrong. Documenting them allows an AI coding agent to flag risks.

**Recommendation:** Add assumptions section.

**Proposed Revision:**

```markdown
## 2.5 Assumptions

| ID | Assumption | Risk if Wrong |
|----|------------|---------------|
| A-001 | GLM 4.7 Flash API is available and stable | High - core feature depends on it |
| A-002 | Users have reliable internet for AI generation | Medium - fallback needed |
| A-003 | Exercise catalog is static (admin-managed) | Low - can add user submissions later |
| A-004 | Single-language (English) for MVP | Low - i18n deferred |
| A-005 | No payment processing in MVP | Low - subscription is feature-flagged |
| A-006 | Team has ASP.NET Core expertise | Medium - onboarding needed if wrong |
```

---

#### Issue 2.6: No Acceptance Criteria

**Why It Matters:** The milestones document has "Test" fields but these are implementation-level checks, not acceptance criteria. An AI coding agent needs acceptance criteria to know when a feature is complete.

**Recommendation:** Add acceptance criteria to key features.

**Proposed Revision (example):**

```markdown
## 2.6 Acceptance Criteria

### AC-AUTH-001: User Registration
- Given a user provides valid email and password
- When they submit the registration form
- Then a verification email is sent
- And the user can log in after email verification
- And the password is stored as a bcrypt hash
- And a JWT access token (15 min) and refresh token (7 days) are issued

### AC-WORKOUT-001: Manual Workout Plan Creation
- Given an authenticated user
- When they create a workout plan with title, type, days/week, and weeks
- Then the plan is saved with status "draft"
- And the user can add sessions to each day
- And each session can include exercises from the catalog

### AC-AI-001: AI Workout Generation
- Given an authenticated user with profile preferences
- When they request AI workout generation
- Then the request is queued asynchronously
- And the user sees a progress indicator
- When generation completes
- Then the workout plan is saved with status "active"
- And the user is notified via WebSocket
- If the AI fails, a template workout is returned
```

---

## CRITERION 3: DOMAIN DESIGN

### Issues Found

#### Issue 3.1: No Domain Glossary

**Why It Matters:** This is the most critical gap. The documentation uses terms inconsistently, which will cause an AI coding agent to make wrong assumptions about entity relationships.

**Recommendation:** Create a comprehensive domain glossary.

**Proposed Revision:**

```markdown
## 3.1 Domain Glossary

| Term | Definition | Key Attributes |
|------|------------|----------------|
| **User** | A registered account holder. Owns workout plans, progress logs, and profile data. | id, email, passwordHash, role, profile, createdAt |
| **Workout Plan** | A structured program spanning multiple weeks. Created manually or via AI. Contains ordered workout sessions. | id, userId, planType, title, description, daysPerWeek, totalWeeks, status, generatedBy |
| **Workout Session** (a.k.a. "Workout") | A single training day within a plan. Belongs to exactly one Workout Plan. Contains one or more Exercise Sets. | id, planId, dayOfWeek, date, title, orderIndex, durationMinutes, completed |
| **Exercise** | A catalog entry representing a physical movement (e.g., "Barbell Bench Press"). Admin-managed. Not user-specific. | id, name, category, difficulty, equipment, instructions, muscleGroup |
| **Exercise Set** | A single set within a Workout Session. References an Exercise. Records reps, weight, and completion. | id, workoutId, exerciseId, setNumber, reps, weightKg, completed |
| **Exercise Variation** | A many-to-many relationship between exercises (e.g., "Bench Press" -> "Incline Bench Press"). | exerciseId, variationId |
| **Progress Log** | A user-recorded measurement entry. Can reference a Workout and Exercise. Used for analytics. | id, userId, workoutId, exerciseId, measurementDate, weightKg, reps, sets |
| **Subscription** | A user's access tier. Controls feature availability (free vs premium). NOT tied to a workout plan. | id, userId, planTier, status, startedAt, expiresAt |
| **Category** | A classification for exercises (e.g., "chest", "back", "legs"). | name, description |
| **AI Recommendation** | A workout plan generated by the AI service. Stored as a Workout Plan with generatedBy='ai'. | (same as Workout Plan) |
```

---

#### Issue 3.2: Ambiguous Terminology - "Workout" vs "Workout Session" vs "Workout Plan"

**Why It Matters:** The architecture document uses these terms interchangeably in places. The API design section uses `/workouts/{id}/workouts` which is confusing.

**Current State:**
- `workout_plans` table = the program
- `workouts` table = daily sessions within a plan
- API: `/workouts/{id}/workouts` = sessions for a plan

**Recommendation:** Standardize terminology:
- **Workout Plan** = the multi-week program
- **Workout Session** = a single training day
- **Exercise Set** = a single set within a session

Update all API paths to use consistent naming:

```markdown
### Revised API Naming

| Path | Resource |
|------|----------|
| `/workout-plans` | Workout Plans (programs) |
| `/workout-plans/{id}/sessions` | Workout Sessions within a plan |
| `/sessions/{id}/sets` | Exercise Sets within a session |
| `/exercises` | Exercise catalog |
| `/progress` | Progress logs |
```

---

#### Issue 3.3: "Subscription" Entity is Misconceived

**Why It Matters:** The current `subscriptions` table links a user to a workout plan, which makes no sense. A subscription should control access tiers, not tie to specific content.

**Current State (from doc2.txt line 290-298):**

```sql
CREATE TABLE subscriptions (
    ...
    plan_id UUID REFERENCES workout_plans(id) ON DELETE CASCADE,
    ...
);
```

**Recommendation:** Redefine Subscription as an access tier model.

**Proposed Revision:**

```markdown
### Subscription (Revised)

A Subscription represents a user's access tier and billing status.

| Field | Type | Description |
|-------|------|-------------|
| id | UUID | Primary key |
| userId | UUID | FK to users, unique (one subscription per user) |
| tier | ENUM | 'free', 'premium', 'pro' |
| status | ENUM | 'active', 'cancelled', 'expired', 'trial' |
| startedAt | TIMESTAMPTZ | When subscription began |
| expiresAt | TIMESTAMPTZ | When subscription ends (null for free tier) |
| paymentProvider | STRING | 'stripe', 'manual', null (for free) |
| externalSubscriptionId | STRING | Provider's subscription ID |
| createdAt | TIMESTAMPTZ | Record creation |
| updatedAt | TIMESTAMPTZ | Last modification |

Business Rules:
- Each user has exactly one subscription record
- Free tier has no expiration
- Free tier limits: 2 AI generations/hour, community support
- Pro tier limits: unlimited AI generations, advanced analytics
```

---

#### Issue 3.4: No Aggregate Roots Defined

**Why It Matters:** Aggregate roots determine transaction boundaries. An AI coding agent needs to know which entities can be modified together in a single transaction.

**Recommendation:** Define aggregate roots.

**Proposed Revision:**

```markdown
## 3.3 Aggregate Roots

| Aggregate Root | Owned Entities | Transaction Boundary |
|----------------|----------------|----------------------|
| **User** | Profile, Subscription | User registration, profile update, subscription change |
| **WorkoutPlan** | WorkoutSession, ExerciseSet | Plan creation, session update, set logging |
| **Exercise** | ExerciseVariation | Admin catalog management |
| **ProgressLog** | (none - leaf entity) | Progress entry |

Rules:
- An ExerciseSet can only be modified within its parent WorkoutSession transaction
- WorkoutPlan deletion cascades to sessions and sets
- Exercise deletion is blocked if referenced by any WorkoutSession
- ProgressLog is append-only (no updates, no deletes)
```

---

#### Issue 3.5: No Entity Lifecycle Definitions

**Why It Matters:** Entities have state transitions that affect behavior. An AI coding agent needs to know valid states and transitions.

**Recommendation:** Define lifecycle state machines.

**Proposed Revision:**

```markdown
## 3.4 Entity Lifecycle

### Workout Plan States

draft -> active -> completed
  |        |
paused   paused
  |        |
active   completed

- `draft`: Plan created, not yet started. Can be edited freely.
- `active`: Plan is in use. Sessions are being logged.
- `paused`: Plan temporarily suspended. Can resume.
- `completed`: All sessions finished. Read-only.

### Workout Session States

scheduled -> in_progress -> completed

- `scheduled`: Session planned for a future date.
- `in_progress`: User has started logging sets.
- `completed`: All sets logged and marked complete.

### Subscription States

trial -> active -> expired
            |
        cancelled -> expired

- `trial`: Free trial period (14 days).
- `active`: Paid subscription in good standing.
- `cancelled`: User cancelled. Remains active until expiration.
- `expired`: Subscription ended. Features downgraded.
```

---

## CRITERION 4: ARCHITECTURE

### Issues Found

#### Issue 4.1: Microservices Diagram Contradicts Modular Monolith Decision

**Why It Matters:** The architecture diagram (section 1.1) shows 6 separate services (Workout API, AI Service, User Service, Analytics API, Payment API, Admin API) which implies microservices. However, section 2 explicitly states the pattern is a "Modular Monolith." This contradiction will confuse an AI coding agent.

**Current State:**
- Section 1.1 diagram shows separate service boxes
- Section 2 says "Modular Monolith with Service-oriented Boundaries"
- Section 11 shows Docker Compose with a single `api` service

**Recommendation:** Update the architecture diagram to reflect a modular monolith.

**Proposed Revision:**

```markdown
## 1.1 High-Level Architecture

```
+-------------------------------------------+
|              Client Layer                  |
|  +------------+    +------------+          |
|  | React SPA  |    | React SPA  |          |
|  | (Web/App)  |    | (Admin)    |          |
|  +------------+    +------------+          |
+-------------------------------------------+
                    |
                    v
+-------------------------------------------+
|          API Gateway (Nginx)                |
|  +----------+ +----------+ +----------+   |
|  |Rate Limit| | SSL Term.| |CDN Cache |   |
|  +----------+ +----------+ +----------+   |
+-------------------------------------------+
                    |
                    v
+-------------------------------------------+
|     ASP.NET Core Modular Monolith          |
|  +-------------------------------------+  |
|  |          Module Boundaries            |  |
|  | +--------+ +--------+ +--------+    |  |
|  | | Auth   | |Workout | |  AI    |    |  |
|  | | Module | | Module | | Module |    |  |
|  | +--------+ +--------+ +--------+    |  |
|  | +--------+ +--------+               |  |
|  | |Progress| | Admin  |               |  |
|  | | Module | | Module |               |  |
|  | +--------+ +--------+               |  |
|  +-------------------------------------+  |
|  +-------------------------------------+  |
|  |       Shared Infrastructure           |  |
|  | +----------+ +----------+ +--------+ |  |
|  | |   Auth   | | Caching  | |Messaging| |  |
|  | | Service  | | Service  | | Service | |  |
|  | +----------+ +----------+ +--------+ |  |
|  +-------------------------------------+  |
+-------------------------------------------+
                    |
                    v
+-------------------------------------------+
|          Data & Support Layer              |
|  +--------+ +--------+ +--------+        |
|  |Postgres| | Redis  | |RabbitMQ|        |
|  |(Primary)| | (Cache) | |(Queue) |        |
|  +--------+ +--------+ +--------+        |
+-------------------------------------------+
```
```

---

#### Issue 4.2: No Module Dependency Rules

**Why It Matters:** In a modular monolith, module dependencies must be explicit. Without rules, an AI coding agent may create circular dependencies between modules.

**Recommendation:** Define module dependency rules.

**Proposed Revision:**

```markdown
## 4.1 Module Dependency Rules

### Dependency Direction

Auth Module -> (no dependencies, leaf module)
Workout Module -> Auth Module (for user context)
AI Module -> Auth Module, Workout Module (generates plans)
Progress Module -> Auth Module, Workout Module (reads sessions)
Admin Module -> Auth Module (user management)

### Forbidden Dependencies
- Auth Module must NOT depend on any other module
- No circular dependencies between modules
- Modules must NOT access each other's database entities directly
- Inter-module communication must use interfaces (abstractions)

### Shared Kernel
The following are shared across all modules:
- Domain primitives (ValueObjects, BaseEntities)
- Authentication/Authorization infrastructure
- Caching infrastructure
- Messaging infrastructure
- Logging infrastructure
- Common DTOs and validators
```

---

#### Issue 4.3: No Event-Driven Architecture for Domain Events

**Why It Matters:** The system has natural domain events (workout completed, progress logged, plan generated) that should trigger side effects. Without event definitions, an AI coding agent will hardcode side effects.

**Recommendation:** Define domain events.

**Proposed Revision:**

```markdown
## 4.2 Domain Events

| Event | Trigger | Handlers |
|-------|---------|----------|
| UserRegistered | POST /auth/register | Send verification email, create default subscription |
| UserVerified | Email verification link clicked | Activate account |
| WorkoutPlanCreated | POST /workouts | Update user workout count |
| WorkoutPlanGenerated | AI generation completes | Notify user, invalidate cache |
| WorkoutSessionCompleted | POST /sessions/{id}/complete | Update plan progress, recalculate stats |
| ExerciseSetLogged | POST /sessions/{id}/sets | Update session progress, check completion |
| ProgressLogged | POST /progress/log | Update analytics cache, check PRs |
| SubscriptionChanged | Subscription state change | Update feature flags, notify user |
```

---

## CRITERION 5: DATABASE READINESS

### Issues Found

#### Issue 5.1: Missing User Profile Entity

**Why It Matters:** The `users` table has a `profile JSONB` column, but the profile structure is undefined. An AI coding agent will create an ad-hoc structure.

**Recommendation:** Define the profile structure explicitly.

**Proposed Revision:**

```markdown
### User Profile Structure

The `profile` JSONB column in `users` contains:

{
  "firstName": "string (required)",
  "lastName": "string (required)",
  "dateOfBirth": "ISO 8601 date",
  "gender": "male | female | other | prefer_not_to_say",
  "heightCm": "number (50-300)",
  "weightKg": "number (20-500)",
  "fitnessLevel": "beginner | intermediate | advanced",
  "goals": ["strength", "hypertrophy", "endurance", "weight_loss", "flexibility"],
  "equipmentAvailable": ["barbell", "dumbbells", "machines", "bodyweight", "bands"],
  "limitations": "string (optional, injuries/restrictions)",
  "avatarUrl": "string (optional)"
}

Validation Rules:
- firstName: 1-50 characters, alpha + spaces
- lastName: 1-50 characters, alpha + spaces
- dateOfBirth: Must be 13+ years old
- heightCm: 50-300
- weightKg: 20-500
- fitnessLevel: Required for AI generation
- goals: At least 1 goal required for AI generation
```

---

#### Issue 5.2: Missing AI Metadata for Generated Plans

**Why It Matters:** AI-generated plans need metadata to track generation parameters. Without this, the system cannot regenerate or adapt plans.

**Recommendation:** Add AI metadata to workout_plans.

**Proposed Revision:**

```markdown
### Workout Plan - AI Metadata

Add `ai_metadata` JSONB column to `workout_plans`:

{
  "modelVersion": "glm-4.7-flash",
  "promptVersion": "v1.2",
  "generationId": "uuid (idempotency key)",
  "inputPreferences": {
    "userLevel": "intermediate",
    "goal": "hypertrophy",
    "equipment": ["barbell", "dumbbells"],
    "daysPerWeek": 4,
    "focusAreas": ["chest", "back", "legs"]
  },
  "tokenUsage": {
    "promptTokens": 1200,
    "completionTokens": 3500,
    "totalTokens": 4700
  },
  "latencyMs": 12500,
  "generatedAt": "2026-07-28T10:30:00Z"
}
```

---

#### Issue 5.3: Missing Audit Trail

**Why It Matters:** For security and debugging, all data modifications should be tracked. Without audit trails, an AI coding agent will not implement them.

**Recommendation:** Add audit columns to all mutable tables.

**Proposed Revision:**

```markdown
### Audit Trail Strategy

Add to all mutable tables:

| Column | Type | Description |
|--------|------|-------------|
| created_by | UUID | FK to users who created the record |
| updated_by | UUID | FK to users who last modified the record |
| created_at | TIMESTAMPTZ | Creation timestamp |
| updated_at | TIMESTAMPTZ | Last modification timestamp |
| is_deleted | BOOLEAN | Soft delete flag (default false) |
| deleted_at | TIMESTAMPTZ | When soft-deleted |
| deleted_by | UUID | Who soft-deleted |
```

---

#### Issue 5.4: No Soft Delete Pattern Defined

**Why It Matters:** The milestones mention "soft delete" in business rules but the schema uses `ON DELETE CASCADE`. This is contradictory.

**Recommendation:** Define soft delete policy.

**Proposed Revision:**

```markdown
### Soft Delete Policy

| Entity | Soft Delete | Hard Delete | Retention |
|--------|-------------|-------------|-----------|
| User | Yes | No (anonymize) | Indefinite |
| Workout Plan | Yes | After 30 days | 30 days |
| Workout Session | Yes (cascade from plan) | After 30 days | 30 days |
| Exercise Set | No (cascade from session) | On session delete | - |
| Exercise | No (admin-managed) | Never | Indefinite |
| Progress Log | No (append-only) | Never | Indefinite |
| Subscription | No (archive) | Never | Indefinite |
```

---

#### Issue 5.5: Missing Indexes Specification

**Why It Matters:** The architecture mentions indexes but does not specify them completely.

**Recommendation:** Define all required indexes.

**Proposed Revision:**

```markdown
### Required Indexes

| Table | Index | Columns | Purpose |
|-------|-------|---------|---------|
| users | idx_users_email | email (unique) | Login lookup |
| workout_plans | idx_wp_user_id | user_id | User's plans |
| workout_plans | idx_wp_user_status | user_id, status | Compound query |
| workouts | idx_w_plan_id | plan_id | Sessions per plan |
| workouts | idx_w_date | date | Date range queries |
| exercise_sets | idx_es_workout_id | workout_id | Sets per session |
| exercise_sets | idx_es_exercise_id | exercise_id | Exercise usage |
| progress_log | idx_pl_user_id | user_id | User's progress |
| progress_log | idx_pl_user_date | user_id, measurement_date | Time-series queries |
| exercises | idx_ex_category | category | Category filter |
| subscriptions | idx_sub_user_id | user_id (unique) | User subscription lookup |
```

---

## CRITERION 6: API READINESS

### Issues Found

#### Issue 6.1: No Request/Response Models

**Why It Matters:** API endpoints are listed but without request body or response body schemas. An AI coding agent will invent these.

**Recommendation:** Define request/response models for all endpoints.

**Proposed Revision (examples):**

```markdown
## 6.1 API Models

### POST /auth/register

**Request:**
{
  "email": "string (required, valid email format)",
  "password": "string (required, 8-128 chars, must include upper, lower, number)",
  "firstName": "string (required, 1-50 chars)",
  "lastName": "string (required, 1-50 chars)"
}

**Response (201):**
{
  "id": "uuid",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "createdAt": "ISO 8601"
}

**Error Responses:**
- 400: Validation error (details in body)
- 409: Email already exists

---

### POST /workouts/{id}/generate

**Request:**
{
  "fitnessLevel": "enum: beginner | intermediate | advanced",
  "goals": ["enum array: strength | hypertrophy | endurance | weight_loss | flexibility"],
  "equipment": ["string array: barbell | dumbbells | machines | bodyweight | bands"],
  "focusAreas": ["string array: chest | back | legs | shoulders | arms | core"],
  "daysPerWeek": "integer (1-7)",
  "totalWeeks": "integer (1-52)",
  "limitations": "string (optional, max 500 chars)"
}

**Response (202 Accepted):**
{
  "generationId": "uuid",
  "status": "queued",
  "estimatedCompletionSeconds": 30,
  "websocketChannel": "ws://api/generations/{generationId}"
}
```

---

#### Issue 6.2: No Pagination Format Defined

**Why It Matters:** Multiple endpoints mention pagination but the format is undefined.

**Recommendation:** Define a standard pagination format.

**Proposed Revision:**

```markdown
## 6.2 Pagination Format

### Standard Paginated Response

{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNext": true,
    "hasPrevious": false
  }
}

### Query Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number (1-indexed) |
| pageSize | integer | 20 | Items per page (max 100) |
| sortBy | string | createdAt | Sort field |
| sortOrder | enum | desc | asc or desc |
| search | string | - | Full-text search term |
| status | string | - | Filter by status |
| type | string | - | Filter by type |
```

---

#### Issue 6.3: No Error Response Format

**Why It Matters:** Error responses are critical for frontend integration. Without a standard format, each endpoint will return different error shapes.

**Recommendation:** Define standard error format.

**Proposed Revision:**

```markdown
## 6.3 Error Response Format

### Standard Error Response

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
    "requestId": "uuid",
    "timestamp": "ISO 8601"
  }
}

### Error Codes

| HTTP Status | Code | When |
|-------------|------|------|
| 400 | VALIDATION_ERROR | Request body fails validation |
| 401 | UNAUTHORIZED | Missing or invalid token |
| 403 | FORBIDDEN | Insufficient permissions |
| 404 | NOT_FOUND | Resource does not exist |
| 409 | CONFLICT | Resource already exists |
| 429 | RATE_LIMITED | Too many requests |
| 500 | INTERNAL_ERROR | Server error |
| 502 | BAD_GATEWAY | Upstream service (GLM API) error |
| 504 | GATEWAY_TIMEOUT | Upstream service timeout |
```

---

#### Issue 6.4: No Authentication Requirements Per Endpoint

**Why It Matters:** Some endpoints are public, some require auth, some require admin. Without this, an AI coding agent will either lock down everything or leave everything open.

**Recommendation:** Define auth requirements per endpoint.

**Proposed Revision:**

```markdown
## 6.4 Authentication Requirements

| Endpoint | Auth Required | Role Required | Rate Limit |
|----------|---------------|---------------|------------|
| POST /auth/register | No | - | 5/min/IP |
| POST /auth/login | No | - | 10/min/IP |
| POST /auth/refresh-token | Yes (refresh token) | - | 60/min |
| GET /auth/profile | Yes | user | 60/min |
| PUT /auth/profile | Yes | user | 30/min |
| GET /users | Yes | admin | unlimited |
| GET /workouts | Yes | user | 60/min |
| POST /workouts | Yes | user | 30/min |
| PUT /workouts/{id} | Yes | user (owner) | 30/min |
| DELETE /workouts/{id} | Yes | user (owner) | 10/min |
| POST /workouts/{id}/generate | Yes | user (owner) | 2/hour |
| GET /exercises | No | - | 60/min/IP |
| POST /exercises | Yes | admin | unlimited |
| GET /progress | Yes | user | 60/min |
| POST /progress/log | Yes | user | 30/min |
```

---

## CRITERION 7: FRONTEND READINESS

### Issues Found

#### Issue 7.1: No Navigation Structure

**Why It Matters:** The frontend folder structure is defined but the navigation hierarchy is not. An AI coding agent needs to know the complete navigation tree.

**Recommendation:** Define navigation structure.

**Proposed Revision:**

```markdown
## 7.1 Navigation Structure

### Primary Navigation (Sidebar)

+---------------------+
|  FitForge AI        |
+---------------------+
|  Dashboard          |
|  My Workouts        |
|    Workout Plans    |
|    Generate Plan    |
|    Workout History  |
|  Progress           |
|    Overview         |
|    Charts           |
|    History          |
|  Exercise Library   |
|  Settings           |
|    Profile          |
|    Account          |
|    Preferences      |
+---------------------+
|  [Upgrade to Pro]   |
+---------------------+

### Route Map

| Route | Page | Auth | Description |
|-------|------|------|-------------|
| / | Landing | No | Marketing page |
| /login | Login | No | Login form |
| /register | Register | No | Registration form |
| /dashboard | Dashboard | Yes | Overview of workouts, stats |
| /workouts | Workout Plans | Yes | List of user's plans |
| /workouts/new | Create Plan | Yes | Manual plan creation |
| /workouts/:id | Plan Detail | Yes | Plan with sessions |
| /workouts/generate | Generate | Yes | AI generation form |
| /progress | Progress | Yes | Charts and metrics |
| /exercises | Exercise Library | Yes | Browse exercises |
| /settings | Settings | Yes | Account settings |
| /admin/users | User Management | Admin | User list |
```

---

#### Issue 7.2: No Design System Specification

**Why It Matters:** Without design system rules, an AI coding agent will create inconsistent UI components.

**Recommendation:** Define design system fundamentals.

**Proposed Revision:**

```markdown
## 7.2 Design System

### Color Palette

| Token | Value | Usage |
|-------|-------|-------|
| primary | #3B82F6 | Buttons, links, active states |
| primary-dark | #1D4ED8 | Hover states |
| secondary | #10B981 | Success, completion |
| warning | #F59E0B | Warnings, in-progress |
| danger | #EF4444 | Errors, delete actions |
| neutral-50 | #F9FAFB | Background |
| neutral-100 | #F3F4F6 | Card backgrounds |
| neutral-200 | #E5E7EB | Borders |
| neutral-500 | #6B7280 | Secondary text |
| neutral-900 | #111827 | Primary text |

### Typography

| Element | Font | Size | Weight |
|---------|------|------|--------|
| H1 | Inter | 30px | 700 |
| H2 | Inter | 24px | 600 |
| H3 | Inter | 20px | 600 |
| Body | Inter | 16px | 400 |
| Small | Inter | 14px | 400 |
| Caption | Inter | 12px | 400 |

### Component Variants

**Button:**
- primary: Blue fill, white text
- secondary: Gray fill, dark text
- danger: Red fill, white text
- ghost: Transparent, blue text
- Sizes: sm (32px), md (40px), lg (48px)

**Card:**
- White bg, rounded-lg, shadow-sm
- Padding: 16px (md), 24px (lg)
- Hover: shadow-md (interactive cards)
```

---

#### Issue 7.3: No Responsive Behavior Defined

**Why It Matters:** The system targets both desktop and mobile browsers. Without responsive rules, components will be inconsistent.

**Recommendation:** Define responsive breakpoints and behaviors.

**Proposed Revision:**

```markdown
## 7.3 Responsive Behavior

### Breakpoints

| Name | Min Width | Target |
|------|-----------|--------|
| mobile | 0px | Phones |
| tablet | 768px | Tablets, small laptops |
| desktop | 1024px | Desktops |
| wide | 1280px | Large screens |

### Layout Changes

| Component | Mobile | Tablet | Desktop |
|-----------|--------|--------|---------|
| Sidebar | Bottom nav (5 items) | Collapsible sidebar | Fixed sidebar |
| Workout List | Single column | 2 columns | 3 columns |
| Charts | Full width, stacked | 2 charts side by side | 3 charts side by side |
| Forms | Full width | Centered (max 600px) | Centered (max 600px) |
```

---

## CRITERION 8: AI INTEGRATION

### Issues Found

#### Issue 8.1: No JSON Schema for AI Output

**Why It Matters:** The prompt template shows the expected JSON structure but there's no formal schema. An AI coding agent needs a schema for validation.

**Recommendation:** Define a formal JSON schema for AI output.

---

#### Issue 8.2: No Token Optimization Strategy

**Why It Matters:** GLM API costs depend on token usage. Without optimization, costs can spiral.

**Recommendation:** Define token optimization strategy.

**Proposed Revision:**

```markdown
## 8.2 Token Optimization

### Cost Management

| Metric | Target | Action if Exceeded |
|--------|--------|-------------------|
| Tokens per generation | < 5,000 | Truncate prompt context |
| Cost per generation | < $0.05 | Cache more aggressively |
| Monthly AI cost | < $500 | Throttle non-premium users |
| Cache hit rate | > 70% | Extend cache TTL |

### Caching Strategy

- Cache key: SHA256(user preferences + plan type)
- Cache TTL: 7 days
- Cache location: Redis
- Invalidation: On user preference change
```

---

#### Issue 8.3: No Model Versioning/Replacement Strategy

**Why It Matters:** AI models evolve. The system must handle model upgrades without downtime.

**Recommendation:** Define model versioning strategy.

---

#### Issue 8.4: No AI Quality Metrics

**Why It Matters:** Without measuring AI output quality, the system cannot improve.

**Recommendation:** Define quality metrics.

**Proposed Revision:**

```markdown
## 8.4 AI Quality Metrics

| Metric | Definition | Target |
|--------|-----------|--------|
| Validation Pass Rate | % of generated plans that pass schema validation | > 95% |
| Exercise Match Rate | % of exercises that match catalog | > 90% |
| User Acceptance Rate | % of generated plans users keep (don't delete) | > 70% |
| Regeneration Rate | % of users who regenerate immediately | < 20% |
| Fallback Rate | % of requests hitting fallback | < 5% |
```

---

## CRITERION 9: CODING STANDARDS

### Issues Found

#### Issue 9.1: No Naming Conventions

**Why It Matters:** Without naming conventions, an AI coding agent will produce inconsistent code.

**Recommendation:** Define naming conventions.

---

#### Issue 9.2: No Backend Folder Structure

**Why It Matters:** The frontend folder structure is defined but the backend structure is not.

**Recommendation:** Define backend folder structure.

---

#### Issue 9.3: No Error Handling Patterns

**Why It Matters:** Without consistent error handling, each developer (or AI agent) will handle errors differently.

**Recommendation:** Define error handling patterns using Result pattern.

---

#### Issue 9.4: No Testing Strategy

**Why It Matters:** The milestones mention testing but don't define the strategy.

**Recommendation:** Define testing strategy with test pyramid.

---

#### Issue 9.5: No Commit Convention

**Why It Matters:** Without commit conventions, git history becomes unreadable.

**Recommendation:** Define Conventional Commits standard.

---

## CRITERION 10: AI CODING AGENT READINESS

### Issues Found

#### Issue 10.1: No User Personas

**Why It Matters:** An AI coding agent building features needs to know who the user is. Without personas, it cannot make user-centered design decisions.

**Recommendation:** Define user personas (beginner, intermediate, trainer/admin).

---

#### Issue 10.2: No CLAUDE.md Template

**Why It Matters:** The milestones mention creating CLAUDE.md (Task 10.3.2) but don't define its contents. This file is the primary interface with AI coding agents.

**Recommendation:** Define CLAUDE.md contents with project overview, tech stack, commands, rules, and key files.

---

#### Issue 10.3: Hidden Assumptions About Exercise Matching

**Why It Matters:** The AI generates exercise names, but the system has an exercise catalog. The matching logic between AI-generated names and catalog entries is undefined.

**Recommendation:** Define three-tier matching: exact, fuzzy, semantic with fallback.

---

#### Issue 10.4: Missing Workflow Definitions

**Why It Matters:** Key user workflows are not fully defined. An AI coding agent needs complete workflow specifications.

**Recommendation:** Define critical workflows: AI generation, workout logging, progress tracking.

---

#### Issue 10.5: No Environment Configuration

**Why It Matters:** An AI coding agent needs to know all environment variables and configuration.

**Recommendation:** Define all required environment variables for backend and frontend.

---

## FINAL ASSESSMENT

### Score: 4.5 / 10

### Reasoning

| Criterion | Score | Notes |
|-----------|-------|-------|
| Product Planning | 3/10 | Missing vision, goals, scope, MVP definition |
| Requirements Engineering | 2/10 | No FR, NFR, business rules, constraints |
| Domain Design | 3/10 | No glossary, ambiguous terminology, wrong subscription model |
| Architecture | 6/10 | Good pattern choice but diagram contradicts, missing events |
| Database Readiness | 5/10 | Schema exists but missing profile structure, audit, soft delete |
| API Readiness | 4/10 | Endpoints listed but no request/response models, error format |
| Frontend Readiness | 4/10 | Folder structure exists but no navigation, design system, states |
| AI Integration | 5/10 | Workflow defined but no schema, optimization, metrics |
| Coding Standards | 3/10 | Branch strategy only, missing naming, error handling, testing |
| AI Agent Readiness | 3/10 | Missing personas, CLAUDE.md, workflows, environment config |

### Improved Table of Contents

```
1. Product Planning
   1.1 Vision
   1.2 Business Goals
   1.3 Project Scope (In/Out)
   1.4 MVP Definition
   1.5 Success Metrics by Milestone
   1.6 Roadmap

2. Requirements Engineering
   2.1 Functional Requirements
   2.2 Non-Functional Requirements
   2.3 Business Rules
   2.4 Constraints
   2.5 Assumptions
   2.6 Acceptance Criteria
   2.7 Definition of Done

3. Domain Design
   3.1 Domain Glossary
   3.2 Entity Relationship Overview
   3.3 Aggregate Roots
   3.4 Entity Lifecycle
   3.5 Value Objects

4. Architecture
   4.1 High-Level Architecture (Modular Monolith)
   4.2 Module Boundaries & Dependencies
   4.3 Technology Stack & Rationale
   4.4 Domain Events
   4.5 CQRS Strategy (when applicable)

5. Database Design
   5.1 Schema (tables, columns, types)
   5.2 Relationships & Constraints
   5.3 Indexes
   5.4 Audit Trail
   5.5 Soft Delete Policy

6. API Specification
   6.1 Endpoint Reference
   6.2 Request/Response Models
   6.3 Pagination Format
   6.4 Error Response Format
   6.5 Authentication Requirements
   6.6 Rate Limiting

7. Frontend Specification
   7.1 Navigation & Routing
   7.2 Design System
   7.3 Component States
   7.4 Responsive Behavior
   7.5 Loading/Empty/Error Patterns
   7.6 State Management

8. AI Integration
   8.1 AI Output Schema
   8.2 Prompt Management
   8.3 Token Optimization
   8.4 Model Versioning
   8.5 Quality Metrics
   8.6 Fallback Strategy

9. Coding Standards
   9.1 Naming Conventions
   9.2 Folder Structure (Backend + Frontend)
   9.3 Error Handling Patterns
   9.4 Testing Strategy
   9.5 Commit Convention
   9.6 Security Practices

10. AI Agent Configuration
    10.1 User Personas
    10.2 CLAUDE.md Template
    10.3 Environment Configuration
    10.4 Critical Workflows
    10.5 Exercise Matching Strategy

11. Infrastructure
    11.1 Docker Compose
    11.2 CI/CD Pipeline
    11.3 Deployment Architecture
    11.4 Monitoring & Observability

12. Project Management
    12.1 Milestones (with acceptance criteria)
    12.2 Risk Assessment
    12.3 Cost Estimation
```

### Recommended Document Order

Documents that MUST exist before any code is written:

1. **Domain Glossary** (Section 3.1) - Foundation for all terminology
2. **Functional Requirements** (Section 2.1) - What the system must do
3. **Architecture Overview** (Section 4.1) - System structure
4. **Database Schema** (Section 5.1) - Data model
5. **API Specification** (Section 6) - Interface contracts
6. **CLAUDE.md** (Section 10.2) - AI agent entry point
7. **Coding Standards** (Section 9) - Code conventions

Documents that can be developed during implementation:

8. **Frontend Specification** (Section 7) - Can evolve with UI development
9. **AI Integration Details** (Section 8) - Can be refined during AI module development
10. **Infrastructure** (Section 11) - Can be set up alongside development

Documents that can be postponed:

11. **Project Management** (Section 12) - Operational, not blocking
12. **Cost Estimation** - Business decision, not technical

### Recommended Documents Before Code

| Document | Priority | Blocks |
|----------|----------|--------|
| DOMAIN_GLOSSARY.md | P0 | Everything |
| REQUIREMENTS.md | P0 | All features |
| ARCHITECTURE.md | P0 | All modules |
| DATABASE.md | P0 | All data operations |
| API_SPEC.md | P0 | Backend + Frontend integration |
| CLAUDE.md | P0 | AI coding agent efficiency |
| CODING_STANDARDS.md | P0 | Code quality |
| AI_INTEGRATION.md | P1 | AI module |
| FRONTEND_SPEC.md | P1 | UI components |
| INFRASTRUCTURE.md | P2 | Deployment |
