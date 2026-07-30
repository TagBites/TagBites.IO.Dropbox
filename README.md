# TagBites.IO.Dropbox

[![Nuget](https://img.shields.io/nuget/v/TagBites.IO.Dropbox.svg)](https://www.nuget.org/packages/TagBites.IO.Dropbox/)
![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-512BD4)
[![License](https://img.shields.io/github/license/TagBites/TagBites.IO.Dropbox)](https://github.com/TagBites/TagBites.IO.Dropbox/blob/master/LICENSE.md)

Dropbox file system support for [TagBites.IO](https://github.com/TagBites/TagBites.IO), built on `Dropbox.Api`. Browse, read, write and sync a Dropbox account through the same `FileSystem` API used for local disk and other storages.

## Install

```
dotnet add package TagBites.IO.Dropbox
```

Targets `netstandard2.0`. Depends on `Dropbox.Api`.

## Usage

```csharp
using TagBites.IO.Dropbox;

// Long-lived OAuth2 token
var fs = DropboxFileSystem.Create(oauth2Token);

// Or a refresh token with your app's key/secret, so access tokens are renewed automatically
// var fs = DropboxFileSystem.Create(oauth2RefreshToken, appKey, appSecret);

var file = fs.GetFile("/reports/summary.txt");
file.WriteAllText("Hello world!");

var content = file.ReadAllText();
```

## Capabilities

- Asynchronous operations. Synchronous calls run on top of them.
- Metadata: none.
- Listings are paginated by Dropbox and fetched page by page.
- The content hash is Dropbox's own algorithm, not MD5, and is reported as such.

## Links

- [Changelog](https://github.com/TagBites/TagBites.IO.Dropbox/blob/master/CHANGELOG.md)
- [Security policy](https://github.com/TagBites/TagBites.IO.Dropbox/blob/master/SECURITY.md)
- [License (MIT)](https://github.com/TagBites/TagBites.IO.Dropbox/blob/master/LICENSE.md)
