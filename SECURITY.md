# Security Policy

## Supported versions

Security fixes are provided for the latest released version.

| Version | Supported |
|---------|-----------|
| 1.0.x   | ✅        |
| < 1.0   | ❌        |

## Reporting a vulnerability

Please **do not** open a public issue for security problems.

Report vulnerabilities privately through GitHub: **Security → [Report a vulnerability](https://github.com/TagBites/TagBites.IO.Dropbox/security/advisories/new)**.

Include a description, the affected version, and a minimal program that reproduces the issue. We aim to acknowledge reports within a few business days and to release a fix or mitigation as soon as a valid issue is confirmed.

## Security model

This package is a provider for [TagBites.IO](https://github.com/TagBites/TagBites.IO). The core security model - no sandbox, paths are the only limit, advisory permissions, content buffered through the system temporary directory - is described in the [core security policy](https://github.com/TagBites/TagBites.IO/blob/master/SECURITY.md). What follows is specific to this provider.

### Credentials

An OAuth2 access token, or a refresh token together with the app key and app secret. All are held in memory for the lifetime of the file system. A leaked refresh token grants continuing access until it is revoked at the Dropbox end.
