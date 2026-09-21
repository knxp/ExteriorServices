# ExteriorServices Roadmap

This roadmap reflects the current business direction: a private admin-first CRM and operations platform for exterior cleaning and holiday lighting services, with AI-assisted property visualization as a core feature. Public lead capture is intentionally deferred for now, but the API and data model will be structured so it can be added later without rework.

The goal is to stay simple, secure, and measurable. We will build the platform in phased stages, validating each phase before moving to the next.

## Phase 0: Foundation and security baseline

- [x] Project structure and layered architecture are in place.
- [x] API and web app can run and communicate.
- [x] Basic dashboard and health-check foundation exist.
- [x] Establish production-ready security baseline.
- [x] Define environment configuration and secret management.
- [x] Add HTTPS enforcement and secure defaults.
- [x] Confirm database and application secrets are never committed to source control.
- [x] Add admin authentication and authorization foundation.
- [x] Add a local SQLite fallback for development when LocalDB is unavailable, while keeping SQL Server support for deployment environments.
- [x] Validate the admin portal loads successfully at http://localhost:5010/admin/customers.

### Security baseline requirements

- Use environment variables or Azure Key Vault for secrets.
- Enforce HTTPS in production.
- Protect all admin and operational routes with authentication and authorization.
- Keep OpenAI keys, storage keys, and database credentials outside source code.
- Add logging and audit patterns for admin actions and API access.
- Prepare for a later public API layer without compromising the private internal system.

### Validation

- App builds successfully.
- Secrets are loaded from configuration only.
- API and web app run under secure local configuration.
- Admin routes are protected and unauthorized access fails cleanly.
- Local development works without a LocalDB install by falling back to SQLite automatically.

## Phase 1: Private admin customer and property intake

- [x] Create a strong internal admin workflow for adding and updating customers.
- [x] Create a strong internal admin workflow for adding and updating property records.
- [x] Add property association to the customer record and ensure the relationship is explicit.
- [x] Add customer and property DTOs, services, and repositories as needed.
- [x] Add search, filter, and list screens for customer/property records.
- [ ] Add notes, status flags, and update timestamps for operational tracking.
- [ ] Add ability to capture property photo references and related job metadata.

### Business goals

- The admin can enter a customer and property quickly while on-site or on the phone.
- The system becomes the operational record for service intake and follow-up.
- Data is organized in a way that supports future quoting, jobs, and analytics.

### Validation

- Admin can create, retrieve, update, and deactivate customers successfully.
- Admin can attach and manage multiple properties per customer.
- Search and list flows are fast and usable on phone/tablet screens.

## Phase 2: Admin dashboard and CRM workflow

- Build a clean mobile-friendly dashboard for day-to-day operations.
- Add searchable customer list with filters and sorting.
- Add searchable property list and quick access to customer details.
- Add overview cards for active customers, total properties, and recent activity.
- Add internal workflows for customer follow-up and status tracking.
- Add audit logging for admin actions and record updates.

### Requirements

- Admin dashboard is optimized for phone and tablet use.
- Admin workflows are simple enough to use while moving through neighborhoods or appointments.
- The UI is readable, clear, and fast.

### Validation

- Dashboard loads and reflects real data from the database.
- Admin can quickly find customer and property records by name, phone, or address.
- Key actions are logged for accountability.

## Phase 3: Jobs and service tracking foundation

- Add domain entities for `Customer`, `Property`, `Job`, and supporting metadata.
- Add service lifecycle management for jobs and follow-up work.
- Add status tracking for job progression.
- Add notes and admin comments per customer or property.
- Add support for multiple service types without hardcoding unrelated business logic.

### Business goals

- Track work after a lead is accepted or a customer is booked.
- Keep customer/property records tied to jobs and service history.
- Prepare the system for future estimates and scheduling without rework.

### Validation

- Jobs can be created and tracked for a customer/property relationship.
- Status updates are clear and auditable.
- The platform is ready for later quote/estimate and scheduling flows.

## Phase 4: File upload and secure image storage

- Add image upload API for property photos.
- Add validation for file type, size, and dimensions.
- Store uploaded images in secure object storage.
- Use a separate storage container or bucket for uploads.
- Add image records associated to the correct customer/property/job.
- Restrict download access to authenticated or authorized users only.

### Requirements

- No arbitrary user can access uploaded files directly.
- Images are stored securely and are associated to the correct operational record.
- Uploaded assets are retained and auditable.

### Validation

- A valid image uploads successfully.
- Invalid file types or oversized files are rejected.
- Files are stored in the configured secure backend.

## Phase 5: AI visualization workflow

- Add a service layer for AI-assisted exterior visualization.
- Add a workflow for uploaded before-photo + lighting prompt.
- Store the original image and generated result in secure storage.
- Add an image job record with statuses such as `Queued`, `Processing`, `Completed`, and `Failed`.
- Add `POST /api/property-visualizations` or equivalent API endpoint.
- Send the original image to OpenAI for transformation.
- Persist the generated image and return it to the admin web client.

### Business goals

- Admin can upload a home photo and preview the impression of installed lights or decor.
- The system helps sell the service with a visual representation of the finished look.
- We can estimate material needs more intelligently using property imagery.

### Validation

- Upload and process one image from API to AI service successfully.
- Generated image is stored and retrievable.
- Failures are logged and surfaced in a safe, non-sensitive way.

## Phase 6: Mobile-first admin experience and operational polish

- Keep the admin experience optimized for phone and tablet usage.
- Add quick-entry forms for customer intake and property edits.
- Add property photo review and purchased-material estimate workflows.
- Add dashboard cards for conversions, jobs, and recent activity.
- Add internal filtering, sorting, and export-friendly views for admin use.

### Requirements

- The UI should feel natural for someone using it in the field.
- The system should prioritize speed and clarity over complex design.
- The app should support the real business workflow rather than a broad generic CRM interface.

### Validation

- Admin can complete core operational tasks quickly.
- Core screens work well on mobile-size devices.
- The interface supports real-world daily usage.

## Phase 7: Analytics and business reporting

- Add counts for total customers, active customers, open jobs, and recent activity.
- Add dashboard chart data for customer acquisition and job trends.
- Add analytics for property visualizations processed.
- Add reporting for service performance and customer engagement.

### Validation

- Dashboard metrics reflect real records from the database.
- Data is accurate and filtered by the intended time range.
- Reports help make operational decisions, not just display vanity numbers.

## Phase 8: Production hardening and deployment

- Add CI/CD pipeline.
- Add automated tests for admin API flows and AI workflow.
- Add integration tests for customer property flows and image processing.
- Add monitoring, alerting, and health checks.
- Add security scanning and dependency review.
- Add backup/restore plan and retention policy.

### Validation

- Deployment pipeline succeeds.
- App remains secure and observable in production.
- Critical workflows are tested and recoverable.

## Public API design for later

Public lead capture is intentionally deferred, but the architecture should still support it in the future without major rework.

### Recommended approach

- Keep the domain model and application services business-focused and not UI-specific.
- Treat public intake as a separate API surface, not as the main internal pathway.
- Design the system so a future public endpoint can call the same business logic used by the admin app.
- Prepare a `Lead` model and related DTOs even if they are used internally first.
- Separate concerns clearly:
  - internal admin API for operational staff
  - future public API for customer inquiries and form submissions

### Example design direction

- Internal admin routes: `/api/customers`, `/api/properties`, `/api/dashboard`
- Future public routes: `/api/public/leads`, `/api/public/quotes`, `/api/public/visualizations`
- Shared application services: customer, property, job, lead, analytics, AI service orchestration

This gives us a clean path to public access later without making the product dependent on public lead capture upfront.

## Deferred / out of scope for the MVP

- Public website and public lead intake
- Customer portal login
- Multi-location business operations
- Complex invoicing and payment flows
- Scheduling engine
- Subscription billing
- Chat systems
- Large marketing automation platform
- Full CRM beyond what is needed for business operations

## Final product scope

The first version of the app should be built as a secure, business-focused internal operations platform centered on:

- admin customer and property intake
- dashboard analytics and operational tracking
- secure property photo storage
- AI-enhanced exterior visualization previews

Public lead capture can be added later once the internal business process is proven working and secure.
