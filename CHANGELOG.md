# Changelog

All notable changes to this project will be documented in this file.

## [Released] - 2025-10-27 (2025.10.27.191)

### Added

* Arcturus.Repository.EntityFrameworkCore.PostgresSql project - PostgreSQL support for repository pattern ([#111](https://github.com/cloudfy/Arcturus/pull/111))
  * Implementation of Repository<T, TKey> for PostgreSQL.
  * SqlSpecificationEvaluator for applying specifications to queries.
  * README.md for the new project.
* Arcturus.Xunit project - ASP.NET Core testing support ([#97](https://github.com/cloudfy/Arcturus/pull/97))
  * TestHost class and supporting files.
* Upsert functionality to repository abstractions ([#105](https://github.com/cloudfy/Arcturus/pull/105))
  * AddOrUpdate method and AddOrUpdateResult<T> struct.
* Enhanced event handling and patching features ([#103](https://github.com/cloudfy/Arcturus/pull/103))
  * New event types and async handler support.
  * Patch handler retrieval and property caching.

### Changed

* FilterExpressionParser now uses string.Compare for proper string comparisons ([#109](https://github.com/cloudfy/Arcturus/pull/109))
  * Refactored comparison expression creation.
* Type resolution and error handling refactored for event messaging ([#109](https://github.com/cloudfy/Arcturus/pull/109))
  * Stricter validation and new UnprocessableEventException for duplicates.
* Refactored cancellation token usage and SQL transaction handling ([#108](https://github.com/cloudfy/Arcturus/pull/108))
* Project metadata enhanced: icons and descriptions updated for several packages ([#99](https://github.com/cloudfy/Arcturus/pull/99))
* Logging and connection improvements for SQLite and RabbitMQ ([#96](https://github.com/cloudfy/Arcturus/pull/96))
* Specification pattern support and documentation improved ([#73](https://github.com/cloudfy/Arcturus/pull/73), [#72](https://github.com/cloudfy/Arcturus/pull/72))
* Middleware, request handling, and result objects refactored ([#81](https://github.com/cloudfy/Arcturus/pull/81), [#84](https://github.com/cloudfy/Arcturus/pull/84))

### Fixed

* Multiple bug fixes for project packaging, event bus, and specification classes ([#99](https://github.com/cloudfy/Arcturus/pull/99), [#94](https://github.com/cloudfy/Arcturus/pull/94))
* Improved error handling in event messaging and connection logic.

### Removed

* No major removals in this period.

---

## [Released] - 2025-07-22

### Added

* None

### Fixed

* String comparison support for FilterExpressionParser - Added proper string comparison support for gt, ge, lt, le operators using string.Compare method

### Changed

* Enhanced FilterExpressionParser to handle string comparisons properly for comparison operators (gt, ge, lt, le)
* Refactored comparison expression creation into a dedicated CreateComparisonExpression method
* [Breaking change] Renamed Arcturus.Data.Repository* > Arcturus.Repository. Package name is changed as well.

### Removed

* None

## [2025.2.14.61] - 2025-02-14

### Added

* v2025.2.14.61 Arcturus.Extensions.CommandLine

### Fixed

* None

### Changed

* None

### Removed

* None

## [2025.1.15.60] - 2025-01-15

### Added

* v2025.1.15.60 Patchable

### Fixed

* None

### Changed

* None

### Removed

* None
