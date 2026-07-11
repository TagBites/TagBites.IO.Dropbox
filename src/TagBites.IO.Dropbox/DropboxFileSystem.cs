namespace TagBites.IO.Dropbox;

/// <summary>
/// Exposes static methods for creating a Dropbox file system.
/// </summary>
public static class DropboxFileSystem
{
    /// <summary>
    /// Creates a Dropbox file system using a long-lived OAuth2 access token.
    /// </summary>
    /// <param name="oauth2Token">The OAuth2 access token.</param>
    /// <returns>A Dropbox file system contains the procedures that are used to perform file and directory operations.</returns>
    public static FileSystem Create(string oauth2Token) => new FileSystem(new DropboxFileSystemOperations(oauth2Token));
    /// <summary>
    /// Creates a Dropbox file system using an OAuth2 refresh token, so access tokens are renewed automatically.
    /// </summary>
    /// <param name="oauth2RefreshToken">The OAuth2 refresh token.</param>
    /// <param name="appKey">The Dropbox app key.</param>
    /// <param name="appSecret">The Dropbox app secret.</param>
    /// <returns>A Dropbox file system contains the procedures that are used to perform file and directory operations.</returns>
    public static FileSystem Create(string oauth2RefreshToken, string appKey, string appSecret) => new FileSystem(new DropboxFileSystemOperations(oauth2RefreshToken, appKey, appSecret));
}
