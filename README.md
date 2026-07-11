# TagBites.IO.Dropbox

Dropbox file system support for [TagBites.IO](https://github.com/TagBites/TagBites.IO), built on `Dropbox.Api`. Browse, read, write and sync a Dropbox account through the same `FileSystem` API used for local disk and other storages.

## Install

```
dotnet add package TagBites.IO.Dropbox
```

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

## License

See [https://www.tagbites.com/io](https://www.tagbites.com/io) for licensing terms.
