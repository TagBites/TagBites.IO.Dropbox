# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed
- Listing a folder returned only the first page of results; all pages are now fetched.
- `overwrite` was ignored when writing and when moving a file, so an existing file was replaced regardless of the argument.
- The Dropbox content hash was reported as MD5, which it is not.
- Creating a folder swallowed conflict errors, and link info lookup swallowed real errors, so both reported success or absence instead of failing.
- `DropboxFileSystem.Create(oauth2RefreshToken, appKey, appSecret)` passed the app secret twice instead of the app key.

### Changed
- Nullable reference types enabled across the provider.
- Dependency versions raised.

## [1.0.2] - 2024-05-24

### Changed
- Automatic versioning with MinVer.

## [1.0.1] - 2024-05-24

### Added
- First release. Dropbox support for `TagBites.IO`, built on `Dropbox.Api`.

[Unreleased]: https://github.com/TagBites/TagBites.IO.Dropbox/compare/1.0.2...HEAD
[1.0.2]: https://github.com/TagBites/TagBites.IO.Dropbox/compare/1.0.1...1.0.2
[1.0.1]: https://github.com/TagBites/TagBites.IO.Dropbox/releases/tag/1.0.1
