# ExteriorServices Roadmap

Implementation will proceed in small, testable stages. Each stage should build on a passing checkpoint before the next stage begins.

## Baseline

- [x] Solution builds successfully.
- [x] Existing unit test suite passes.
- [x] SQL Server connection is verified.
- [x] Initial EF migration is applied.
- [x] API starts and Swagger is reachable.

## 1. Persistence Cleanup

- [x] Move EF Core mappings into `IEntityTypeConfiguration<T>` classes.
- [x] Keep the existing database schema unchanged.
- [x] Defer the decision on whether `PropertyType` should become an enum-backed value.
- Validation: build the solution and verify there are no pending EF model changes.

## 2. Customer Application Boundary

- [x] Add customer DTOs, request models, and `ICustomerService`.
- [x] Implement list, get, create, update, and soft-deactivate operations.
- [x] Refactor `CustomersController` to depend on the application service only.
- Validation: focused service tests and Swagger checks for every customer route.

## 3. Property Application Boundary

- [x] Add property DTOs, request models, and `IPropertyService`.
- [x] Implement customer-scoped listing and creation, retrieval, and update.
- [x] Enforce that every property belongs to an existing customer.
- Validation: focused relationship tests and Swagger route checks.

## 4. Validation and Error Handling

- [x] Add request validation at the API/application boundary.
- [x] Add centralized exception handling and a consistent error response.
- [x] Log unexpected failures without exposing internal details.
- Validation: malformed requests, missing records, and invalid relationships verified against the running API. Unexpected exceptions are logged and return a generic error response through middleware.

## 5. Dependency Injection Cleanup

- [x] Add `AddApplication()` and `AddInfrastructure()` registration extensions.
- [x] Keep API `Program.cs` focused on composition, middleware, and endpoint mapping.
- Validation: clean build, 8 passing tests, and successful API startup.

## 6. Dashboard Foundation

- Add `DashboardDto`, `IDashboardService`, and `DashboardService`.
- Add `GET /api/dashboard` with total customers, active customers, and total properties.
- Validation: database-backed service/API tests with known counts.

## 7. Web/API Integration

- Add an HTTP client in the Web project.
- Replace the Razor placeholder with API-backed functionality.
- Preserve the Web to HTTP API boundary.
- Validation: Web and API smoke test.

## 8. Test Expansion

- Add focused unit tests for Customer and Property application services.
- Add API/integration tests for endpoint contracts and error responses.
- Run the complete solution test suite after each major milestone.

## Deferred

Leads, service catalog, activities, estimates, jobs, analytics, scheduling, invoicing, payments, AI integrations, mobile clients, and customer portals remain future capabilities. They should use the same application/API boundaries without being introduced during this foundation refactor.
