# Vision

Describe the product vision for ExteriorServices here.

# API Refactor & Foundation Specification

## 1. Purpose

Refactor the current ASP.NET Core API from a basic CRUD implementation into a clean, maintainable business API that will serve as the foundation for the company's CRM and future operations platform.

The software is being built specifically for our company.

Do **not** introduce multi-tenancy, organizations, SaaS concepts, or organization IDs at this stage.

The company may eventually offer multiple service lines, such as:

- Junk Removal
- Christmas Light Installation
- Pressure Washing
- Gutter Cleaning
- Window Cleaning
- Future services

The API should support these as configurable business services rather than requiring a separate controller or architecture for every service.

The primary objective is to establish a strong foundation for:

- Customers
- Properties
- Leads
- Services
- Estimates
- Jobs
- Activities
- Analytics
- Future scheduling
- Future invoicing/payments
- Future AI integrations
- Future mobile/public applications

---

# 2. Target Architecture

Use the following dependency structure:

```text
ExteriorServices.Web
        │
        │ HTTP
        ▼
ExteriorServices.Api
        │
        ▼
ExteriorServices.Application
        │
        ├───────────────┐
        ▼               ▼
ExteriorServices.Domain
        ▲
        │
ExteriorServices.Infrastructure
        │
        ▼
    SQL Server
```

Dependency rules:

```text
Domain
    ↓
Nothing

Application
    ↓
Domain

Infrastructure
    ↓
Application
    ↓
Domain

API
    ↓
Application
    ↓
Infrastructure

Web
    ↓
API through HTTP
```

The Domain layer must not depend on:

- ASP.NET Core
- EF Core
- SQL Server
- Blazor
- HTTP
- AI providers
- external APIs

The Application layer contains business use cases and interfaces.

The Infrastructure layer handles implementation details such as EF Core and SQL Server.

The API layer handles HTTP concerns only.

---

# 3. Core Architectural Principle

The API is the central business boundary.

The Blazor frontend should never directly access:

- DbContext
- EF Core
- SQL Server
- Domain persistence details

Instead:

```text
Blazor
    ↓
HTTP API
    ↓
Application Services
    ↓
Infrastructure
    ↓
SQL Server
```

Future applications should use the same API:

```text
Blazor Dashboard ──┐
Mobile App ────────┤
Public Website ────┤
AI Services ───────┤
                    ▼
                 API
                    │
                    ▼
                 Database
```

The API should therefore remain independent of the current UI.

---

# 4. Refactor Goal

The current controllers directly interact with the DbContext.

Refactor away from this pattern:

```text
Controller
    ↓
DbContext
    ↓
Database
```

toward:

```text
Controller
    ↓
Application Service
    ↓
EF Core / Infrastructure
    ↓
Database
```

Controllers should be thin.

Controllers should primarily:

1. Receive HTTP requests
2. Validate basic request structure
3. Call the appropriate application service
4. Return the appropriate HTTP response

Controllers should NOT contain significant business logic.

---

# 5. Introduce DTOs

Do not expose Domain entities directly through API endpoints.

Create request/response DTOs.

For Customers:

```text
Application/
└── Customers/
    ├── CustomerDto.cs
    ├── CreateCustomerRequest.cs
    ├── UpdateCustomerRequest.cs
    └── CustomerService.cs
```

Example conceptual DTO:

```text
CustomerDto
-----------
Id
FirstName
LastName
Phone
Email
IsActive
CreatedAt
UpdatedAt
```

Create request:

```text
CreateCustomerRequest
---------------------
FirstName
LastName
Phone
Email
```

Update request:

```text
UpdateCustomerRequest
---------------------
FirstName
LastName
Phone
Email
IsActive
```

Do not expose:

- EF navigation properties unnecessarily
- DbContext-specific information
- database implementation details

The API contract should be independent from the database schema.

---

# 6. Application Services

Introduce application services for major foundational concepts.

Initial services:

```text
CustomerService
PropertyService
```

Future services:

```text
LeadService
ServiceCatalogService
EstimateService
JobService
ActivityService
AnalyticsService
```

Do not create services simply because a database table exists.

Services should represent meaningful business operations.

For example:

```text
CustomerService
    GetCustomer
    GetCustomers
    CreateCustomer
    UpdateCustomer
```

Later:

```text
EstimateService
    CreateEstimate
    AddEstimateItem
    UpdateEstimate
    SendEstimate
    AcceptEstimate
    RejectEstimate
```

The Application layer should contain business rules and use-case orchestration.

---

# 7. Interfaces

Where appropriate, define interfaces in the Application layer.

For example:

```text
ICustomerService
IPropertyService
```

The API depends on the interface rather than the implementation.

Conceptually:

```text
CustomersController
        ↓
ICustomerService
        ↓
CustomerService
```

Register implementations through dependency injection.

Do not introduce unnecessary abstractions for every single class.

The purpose is separation of concerns, not abstraction for its own sake.

---

# 8. Customer API

Refactor the current CustomersController.

Target endpoints:

```http
GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
DELETE /api/customers/{id}
```

Deletion behavior should be considered carefully.

For business records, prefer deactivation/soft deletion where appropriate rather than permanently deleting historical customer data.

For example:

```text
IsActive = false
```

can be used instead of physically deleting a customer.

Do not implement complicated soft-delete infrastructure yet.

---

# 9. Property API

Refactor PropertiesController.

Target endpoints:

```http
GET    /api/properties
GET    /api/properties/{id}
GET    /api/customers/{customerId}/properties
POST   /api/customers/{customerId}/properties
PUT    /api/properties/{id}
```

A property belongs to a customer in the current system.

The API should make the customer/property relationship explicit.

Example:

```http
POST /api/customers/123/properties
```

rather than requiring the frontend to manually construct the relationship.

---

# 10. Customer vs Property

Maintain a distinction between:

```text
Customer
```

and:

```text
Property
```

A customer represents the person/business relationship.

A property represents the physical location where services may occur.

This is important because the company may eventually have:

```text
Customer
    │
    ├── Property A
    ├── Property B
    └── Property C
```

Different services may occur at the same property over time.

Do not combine address information directly into Customer.

---

# 11. Service Catalog

Prepare the API architecture for multiple company service lines.

Do NOT create controllers such as:

```text
JunkRemovalController
PressureWashingController
ChristmasLightsController
```

unless a future service develops genuinely unique business operations requiring its own domain logic.

Instead, create a generic service catalog.

Conceptually:

```text
Service
-------
Id
Name
Category
Description
IsActive
```

Examples:

```text
Junk Removal
Pressure Washing
House Wash
Driveway Cleaning
Christmas Light Installation
Gutter Cleaning
Window Cleaning
```

The CRM should treat these as services the company offers.

This allows the company to add/remove services without changing the fundamental API architecture.

---

# 12. Service Categories

Allow services to be grouped.

Example:

```text
Junk Removal
    ├── Residential Junk Removal
    ├── Commercial Junk Removal
    └── Appliance Removal

Exterior Cleaning
    ├── House Wash
    ├── Driveway Cleaning
    ├── Gutter Cleaning
    └── Window Cleaning

Christmas Lighting
    ├── Installation
    ├── Removal
    └── Storage
```

Do not over-engineer the category system initially.

A simple category relationship is sufficient.

---

# 13. Leads

After Customers and Properties are refactored, introduce Leads.

A Lead represents a potential customer before conversion.

Initial conceptual model:

```text
Lead
----
Id
FirstName
LastName
Phone
Email
SourceId
Status
Notes
CreatedAt
UpdatedAt
```

Lead sources should be configurable.

Examples:

```text
Google
Facebook
Referral
Door Knock
Website
Yard Sign
Repeat Customer
Commercial Outreach
Other
```

Do not build AI lead scoring.

Do not build predictive lead analysis.

Do not build automated lead recommendations.

The system should simply capture reliable data.

---

# 14. Lead Conversion

Eventually a lead should be convertible into a Customer.

Conceptually:

```text
Lead
 ↓
Qualified
 ↓
Converted
 ↓
Customer
```

Conversion should be handled by application logic rather than the controller manually creating multiple database records.

For example:

```text
LeadService.ConvertLead()
```

can eventually:

1. Validate the lead
2. Create the customer
3. Transfer relevant contact information
4. Mark the lead as converted
5. Create an activity

Do not fully implement this unless required for the current milestone.

Prepare the architecture for it.

---

# 15. Activities

Introduce a generic customer activity concept.

Activities create the timeline/history of the relationship.

Conceptually:

```text
CustomerActivity
----------------
Id
CustomerId
ActivityType
Title
Description
CreatedAt
CreatedBy
```

Possible activity types:

```text
CustomerCreated
NoteAdded
PhoneCall
Email
EstimateCreated
EstimateAccepted
EstimateRejected
JobScheduled
JobCompleted
PaymentReceived
```

The exact list can evolve.

The activity system should allow the customer detail page to eventually show:

```text
Customer History

Today
    Estimate created
    Phone call

Yesterday
    Note added

Aug 27
    Customer created
```

Activities should be treated as historical records.

Avoid rewriting historical activities.

---

# 16. Estimates

Do not fully implement Estimates during this refactor unless needed for testing architecture.

However, design the API so the future structure is clear:

```text
Estimate
    │
    └── EstimateItems
            │
            └── Service
```

Conceptually:

```text
Estimate
--------
Id
CustomerId
PropertyId
Status
CreatedAt
ExpirationDate
Subtotal
Tax
Total
```

```text
EstimateItem
------------
Id
EstimateId
ServiceId
Description
Quantity
UnitPrice
Total
```

The important concept is that estimates reference services rather than hardcoded service types.

---

# 17. Jobs

Jobs should eventually represent accepted/actual work.

Conceptually:

```text
Estimate
    ↓
Accepted
    ↓
Job
```

A job can reference:

- Customer
- Property
- Services
- Estimate
- Schedule
- Status

Again, do not implement the complete Job system during this API refactor.

Prepare the architecture so it can be added without restructuring Customers or Properties.

---

# 18. Analytics

Analytics should NOT query the database directly from the Blazor frontend.

Eventually:

```http
GET /api/dashboard
GET /api/analytics/revenue
GET /api/analytics/customers
GET /api/analytics/leads
```

The dashboard API should aggregate data for the UI.

Example future response:

```json
{
  "customers": 128,
  "newCustomersThisMonth": 12,
  "openLeads": 23,
  "openEstimates": 14,
  "revenueThisMonth": 18420
}
```

Do not implement advanced analytics yet.

The goal of this refactor is to make future analytics possible.

---

# 19. Dashboard API

Create the conceptual foundation for:

```http
GET /api/dashboard
```

This endpoint should eventually return dashboard-specific aggregated data.

The dashboard should NOT make a dozen unrelated API requests simply to display the homepage.

However, do not implement complex analytics logic inside the controller.

Use an application-level DashboardService.

Conceptually:

```text
DashboardController
        ↓
IDashboardService
        ↓
DashboardService
        ↓
Data access
```

---

# 20. Infrastructure

Keep EF Core implementation inside:

```text
ExteriorServices.Infrastructure
```

The Infrastructure project owns:

- DbContext
- EF Core configuration
- migrations
- database-specific implementation
- future external service implementations

The Application layer should not directly depend on SQL Server.

The Domain layer should not know EF Core exists.

---

# 21. EF Core Configuration

As the number of entities grows, move EF configuration out of the DbContext.

Use:

```text
Infrastructure/
└── Data/
    ├── ExteriorServicesDbContext.cs
    ├── Configurations/
    │   ├── CustomerConfiguration.cs
    │   ├── PropertyConfiguration.cs
    │   └── ...
    └── Migrations/
```

Use:

```csharp
IEntityTypeConfiguration<T>
```

for entity configurations.

The DbContext should become responsible primarily for:

- DbSet declarations
- applying configurations

Avoid allowing the DbContext to become a giant configuration file.

---

# 22. API Error Handling

Introduce centralized API error handling.

Controllers should not repeatedly implement:

```text
try
catch
return BadRequest
return InternalServerError
```

Create centralized exception handling/middleware.

The API should return consistent error responses.

For example:

```json
{
  "error": "CustomerNotFound",
  "message": "The requested customer could not be found."
}
```

Do not expose:

- SQL exceptions
- stack traces
- internal implementation details

to the frontend.

Keep detailed errors in server logs.

---

# 23. Validation

Introduce request validation at the API/application boundary.

Examples:

Customer:

```text
FirstName required
LastName required
Email optional
Phone optional
```

Property:

```text
AddressLine1 required
City required
State required
PostalCode required
Customer must exist
```

Validation should return appropriate `400 Bad Request` responses.

Do not rely solely on SQL Server constraints for user input validation.

---

# 24. Logging

Use the built-in ASP.NET Core logging infrastructure.

Important events should eventually be logged:

- API failures
- customer creation failures
- estimate failures
- unexpected exceptions
- external service failures

Do not log sensitive customer information unnecessarily.

Do not implement a custom logging framework.

---

# 25. Dependency Injection

Centralize registration where practical.

The API's Program.cs should not eventually contain dozens of manually registered services.

Consider extension methods such as:

```text
AddApplication()
AddInfrastructure()
```

Conceptually:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
```

This keeps Program.cs clean as the application grows.

---

# 26. API Naming

Use consistent REST-style naming.

Prefer:

```http
GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
```

Avoid:

```http
GET /api/GetCustomers
POST /api/CreateCustomer
```

Use plural resource names.

Keep route naming consistent across the entire API.

---

# 27. Do Not Create One Controller Per Database Table Automatically

Controllers should represent API resources/business capabilities.

For example:

```text
CustomersController
PropertiesController
LeadsController
EstimatesController
JobsController
DashboardController
```

Do not automatically create:

```text
CustomerActivitiesController
EstimateItemsController
JobItemsController
...
```

unless those entities need independent API operations.

Many child records should be managed through their parent resource.

Example:

```text
POST /api/estimates/{estimateId}/items
```

rather than exposing every database table as a public API resource.

---

# 28. Avoid Generic CRUD Controllers

Do not create something like:

```text
GenericController<T>
```

to automatically CRUD every entity.

This is a business application.

Different resources will eventually have different rules.

Explicit application services/controllers are preferable to clever generic abstractions.

---

# 29. Testing

Create unit tests around Application-layer business logic.

Initial targets:

```text
CustomerService
PropertyService
```

Tests should cover:

- Creating customer
- Updating customer
- Retrieving customer
- Customer not found
- Creating property
- Property requires valid customer

````

Integration/API tests can be added later.

Do not aim for 100% coverage immediately.

Focus on business-critical behavior.

---

# 30. What NOT to build during this refactor

Do not introduce:

- Multi-tenancy
- OrganizationId
- SaaS architecture
- AI
- AI lead scoring
- AI predictions
- Payments
- Stripe
- SMS
- Email automation
- Customer portal
- Mobile application
- Advanced scheduling
- Data warehouse
- TrueNAS integration
- Microservices
- Kubernetes
- Docker infrastructure
- Complex event-driven architecture
- Generic repository framework
- Generic CRUD framework

The purpose of this work is to create a strong foundation, not to build the entire product.

---

# 31. Target Project Structure

After this refactor, the solution should approximately look like:

```text
ExteriorServices/
│
├── src/
│   │
│   ├── ExteriorServices.Api/
│   │   ├── Controllers/
│   │   │   ├── CustomersController.cs
│   │   │   ├── PropertiesController.cs
│   │   │   └── DashboardController.cs
│   │   │
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── ExteriorServices.Application/
│   │   ├── Customers/
│   │   │   ├── ICustomerService.cs
│   │   │   ├── CustomerService.cs
│   │   │   ├── CustomerDto.cs
│   │   │   ├── CreateCustomerRequest.cs
│   │   │   └── UpdateCustomerRequest.cs
│   │   │
│   │   ├── Properties/
│   │   │   ├── IPropertyService.cs
│   │   │   ├── PropertyService.cs
│   │   │   ├── PropertyDto.cs
│   │   │   ├── CreatePropertyRequest.cs
│   │   │   └── UpdatePropertyRequest.cs
│   │   │
│   │   └── Dashboard/
│   │       ├── IDashboardService.cs
│   │       ├── DashboardService.cs
│   │       └── DashboardDto.cs
│   │
│   ├── ExteriorServices.Domain/
│   │   └── Entities/
│   │       ├── Customer.cs
│   │       └── Property.cs
│   │
│   └── ExteriorServices.Infrastructure/
│       └── Data/
│           ├── ExteriorServicesDbContext.cs
│           ├── Configurations/
│           │   ├── CustomerConfiguration.cs
│           │   └── PropertyConfiguration.cs
│           └── Migrations/
│
├── tests/
│   └── ExteriorServices.UnitTests/
│
└── docs/
````

The exact folder organization can change if the agent identifies a better equivalent structure, but the architectural boundaries should remain intact.

---

# 32. Refactor Sequence

Implement this in stages.

## Phase 1 — Clean Architecture Foundation

1. Verify current solution builds.
2. Verify current database connection.
3. Verify current Swagger endpoints.
4. Ensure project references follow the intended dependency direction.
5. Remove unnecessary default template code.

Do not break existing functionality.

---

## Phase 2 — Domain Cleanup

1. Review Customer entity.
2. Review Property entity.
3. Move database-specific concerns out of Domain.
4. Add appropriate navigation properties.
5. Add EF configurations in Infrastructure.

Run migrations only when schema changes are actually required.

---

## Phase 3 — Customer Refactor

1. Create Customer DTOs.
2. Create CreateCustomerRequest.
3. Create UpdateCustomerRequest.
4. Create ICustomerService.
5. Create CustomerService.
6. Refactor CustomersController to use the service.
7. Add update endpoint.
8. Add appropriate validation.
9. Ensure API no longer returns the Domain entity directly.

Test through Swagger.

---

## Phase 4 — Property Refactor

1. Create Property DTOs.
2. Create request models.
3. Create IPropertyService.
4. Create PropertyService.
5. Refactor PropertiesController.
6. Add customer/property routes.
7. Validate customer existence.
8. Ensure Domain entities are not directly returned.

Test through Swagger.

---

## Phase 5 — Centralized Error Handling

1. Add global exception handling middleware.
2. Define consistent API error responses.
3. Remove unnecessary try/catch blocks from controllers.
4. Ensure unexpected exceptions are logged.
5. Ensure internal details are not returned to clients.

---

## Phase 6 — Dependency Injection Cleanup

Create:

```text
AddApplication()
AddInfrastructure()
```

extensions where useful.

Program.cs should become concise and readable.

---

## Phase 7 — Dashboard Foundation

Create:

```text
DashboardController
DashboardService
DashboardDto
```

Initially return basic real database statistics:

```text
TotalCustomers
ActiveCustomers
TotalProperties
```

Do not implement advanced analytics yet.

The purpose is to establish the API contract that the future Blazor dashboard will consume.

---

## Phase 8 — Tests

Add unit tests for:

```text
CustomerService
PropertyService
```

Verify:

- happy paths
- invalid customer/property relationships
- missing records
- update behavior
- validation/business rules

---

# 33. Definition of Done

The API refactor is complete when:

### Architecture

- Domain has no infrastructure dependencies.
- Application contains business/use-case logic.
- Infrastructure contains EF Core/database implementation.
- API contains HTTP concerns.
- Controllers are thin.

### Customers

- CRUD operations are available.
- DTOs are used.
- Application service is used.
- Domain entities are not directly returned.

### Properties

- CRUD operations are available.
- Customer/property relationship is enforced.
- DTOs are used.
- Application service is used.

### Errors

- API has centralized error handling.
- Errors have consistent responses.
- Internal exceptions aren't exposed.

### Database

- EF Core configuration is separated appropriately.
- Existing data remains intact.
- Migrations remain functional.

### Dashboard

- `/api/dashboard` exists.
- Dashboard data comes from the API.
- Basic real database metrics are returned.

### Maintainability

Adding a new business concept such as:

```text
Lead
```

should NOT require restructuring the entire project.

Adding a new service such as:

```text
Junk Removal
Pressure Washing
Christmas Lighting
```

should NOT require creating a new API architecture.

The system should be able to grow by adding new domain/application capabilities within the existing architecture.

---

# 34. Long-Term Architecture Goal

The final system should evolve approximately like this:

```text
                       COMPANY SOFTWARE
                              │
              ┌───────────────┼────────────────┐
              │               │                │
           CRM CORE       OPERATIONS       ANALYTICS
              │               │                │
       ┌──────┼──────┐    ┌───┼────┐           │
       │      │      │    │   │    │           │
   Customers Leads Properties Jobs Estimates  Reports
       │      │      │        │
       └──────┴──────┴────────┘
                    │
                    ▼
                   API
                    │
          ┌─────────┼──────────┐
          │         │          │
       Blazor    Mobile      Website

                    │
                    ▼
               Future AI
```

The important point is that **AI, websites, mobile apps, and specific service-line functionality are consumers/extensions of the core system rather than the foundation itself.**

The core should remain useful regardless of whether the company ultimately focuses on junk removal, pressure washing, Christmas lighting, or multiple services.

---

# 35. Guiding Principle

Build the software around the company's **business relationships and operations**, not around a specific service.

The system should answer:

> Who are our customers?

> Where do they need service?

> How did they find us?

> What have we sold them?

> What work have we performed?

> What have we charged?

> What have we been paid?

> What happened during our relationship with them?

> How is the business performing?

Service-specific functionality should be layered on top of those fundamentals.
