namespace TagBites.IO.Dropbox
{
    public static class DropboxFileSystem
    {
        public static FileSystem Create(string oauth2Token) => new FileSystem(new DropboxFileSystemOperations(oauth2Token));
        public static FileSystem Create(string oauth2RefreshToken, string appKey, string appSecret) => new FileSystem(new DropboxFileSystemOperations(oauth2RefreshToken, appSecret, appSecret));
    }
}
