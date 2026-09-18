# ExteriorServices Roadmap

This roadmap reflects the new product direction: a business focused on exterior cleaning and holiday lighting services, with a public lead-capture interface, an admin portal, and AI-assisted property visualization.

The goal is to stay simple, secure, and measurable. We will implement the platform in phased stages, validating each phase before moving to the next.

## Phase 0: Foundation and security baseline

- [x] Project structure and layered architecture are in place.
- [x] API and web app can run and communicate.
- [x] Basic dashboard and health-check foundation exist.
- [ ] Establish production-ready security baseline.
- [ ] Define environment configuration and secret management.
- [ ] Add HTTPS enforcement and secure defaults.
- [ ] Confirm database and application secrets are never committed to source control.

### Security baseline requirements

- Use environment variables or Azure Key Vault for secrets.
- Enforce HTTPS in production.
- Protect admin and customer routes with authentication and authorization.
- Keep OpenAI keys, storage keys, and database credentials outside source code.
- Add logging and audit patterns for admin actions and API access.

### Validation

- App builds successfully.
- Secrets are loaded from configuration only.
- API and web app run under secure local configuration.

## Phase 1: Public lead capture and contact intake

- Create a public-facing lead intake experience.
- Add a `Lead` domain model and supporting data model.
- Add `LeadDto`, `CreateLeadRequest`, and `LeadStatus`.
- Add `ILeadService` and `LeadService`.
- Add `POST /api/leads` public endpoint for form submissions.
- Add optional `Phone`, `Email`, `Address`, `ServiceType`, `Notes` fields.
- Add a consent field for privacy and communication acceptance.
- Add validation to block spammy or malformed submissions.
- Add rate limiting and bot protection for public access.

### Business goals

- Anyone can submit their contact details and request service info.
- Lead data is captured securely and stored in the DB.
- We can later track each lead through follow-up and quoting.

### Validation

- Submit test lead from the public form.
- Confirm response is successful and stored in database.
- Confirm invalid submissions fail cleanly.
- Confirm rate limiting and validation work.

## Phase 2: Admin authentication and protected portal

- Add admin authentication and authorization.
- Create an admin portal area for secure internal access.
- Add authenticated admin dashboard for analytics and customer data.
- Add admin management for customers and leads.
- Add search, filter, and status update workflows.
- Add audit logging for customer and lead changes.

### Requirements

- Admin routes are protected and require login.
- Only authorized users can view or edit customer and lead records.
- Admin actions must be auditable.

### Validation

- Login fails for unauthorized users.
- Admin pages load only for authenticated users.
- Lead/customer records can be created, updated, and retrieved securely.

## Phase 3: Customer and job management foundation

- Add domain entities for `Customer`, `Property`, `Lead`, and `Job`.
- Add service lifecycle management for jobs and follow-up.
- Add status tracking for lead progression.
- Add customer record creation from approved lead intake.
- Add property association to customer and job records.
- Add notes and admin comments.

### Business goals

- Convert a lead into a customer when appropriate.
- Track service requests and project progress.
- Keep customer/property records organized and auditable.

### Validation

- Lead creation and conversion works end-to-end.
- Customer records are linked to properties and jobs properly.
- Admin can retrieve historical customer/job context.

## Phase 4: File upload and secure image storage

- Add image upload API for property photos.
- Add validation for file type, size, and dimensions.
- Store uploaded images in secure object storage.
- Use a separate storage container or bucket for uploads.
- Add records for uploaded images and associated customer/property/job.
- Restrict download access to authenticated or authorized users only.

### Requirements

- No arbitrary user can access uploaded files directly.
- Images are stored securely and are associated to the correct customer/job.
- Uploaded assets are retained and auditable.

### Validation

- A valid image uploads successfully.
- Invalid file types or oversized files are rejected.
- Files are stored in the configured secure backend.

## Phase 5: AI visualization workflow

- Add a service layer for AI-assisted house visualizations.
- Add a workflow for uploaded before-photo + light rendering prompt.
- Store the original image and generated result in secure storage.
- Add an image job record with statuses such as `Queued`, `Processing`, `Completed`, `Failed`.
- Add `POST /api/property-visualizations` or equivalent API endpoint.
- Send the original image to OpenAI for transformation.
- Persist the generated image and return it to the web client.

### Business goals

- Customer can upload their home photo.
- System produces a visualization showing the property with lights installed.
- Client can review the preview before committing to service.

### Validation

- Upload and process one image from API to AI service successfully.
- Generated image is stored and retrievable.
- Failures are logged and surfaced in a safe, non-sensitive way.

## Phase 6: Public and admin web experiences

- Public page for lead capture and service inquiry.
- Public page for uploading a property photo and viewing generated preview.
- Admin dashboard showing leads, customer count, property count, and analytics.
- Admin pages for customer management, AI job tracking, and audit review.

### Requirements

- Public web UI uses the API but does not expose admin controls.
- Admin UI is separate in access and function.
- Both experiences share the same backend and data model.

### Validation

- Public page submits a valid lead and displays confirmation.
- Admin page lists leads and analytics from secure backend.
- AI preview is visible in the public or admin flow as designed.

## Phase 7: Analytics and business reporting

- Add counts for total leads, active leads, conversion rate, and service category.
- Add dashboard chart data for leads by source and time range.
- Add analytics for property visualizations processed.
- Add reporting for service performance and customer activity.

### Validation

- Dashboard metrics reflect real records from the database.
- Data is accurate and filtered by the intended time range.

## Phase 8: Production hardening and deployment

- Add CI/CD pipeline.
- Add automated tests for public API, admin API, and image generation flow.
- Add integration tests for lead submission and AI workflow.
- Add monitoring, alerting, and health checks.
- Add security scanning and dependency review.
- Add backup/restore plan and retention policy.

### Validation

- Deployment pipeline succeeds.
- App remains secure and observable in production.
- Critical workflows are tested and recoverable.

## Deferred / out of scope for the MVP

- Multi-location business operations
- Mobile app
- Complex invoicing and payment flows
- Customer portal login
- Scheduling engine
- Subscription billing
- Chat systems
- Large marketing automation platform
- Full CRM beyond what is needed for business operations

The product should stay focused on the three actual goals:

1. Public lead capture
2. Admin analytics and customer management
3. AI-powered property visualization previews

## Final product scope

The app should be built as a secure, business-focused platform centered on:

- lead capture from the public web
- admin analytics and customer records
- AI-enhanced exterior visualization

Everything else should be added only after the core value flow is working reliably and securely.
