using System;
using Xunit;

namespace TagBites.IO.Dropbox.Tests;

public class DropboxFileSystemTests
{
    [Fact]
    public void Create_WithAccessToken_KindIsDropbox()
    {
        var fileSystem = DropboxFileSystem.Create("access-token");

        Assert.Equal("dropbox", fileSystem.Kind);
    }

    [Fact]
    public void Create_NullAccessToken_Throws()
    {
        Assert.Throws<ArgumentException>(() => DropboxFileSystem.Create(null!));
    }

    [Fact]
    public void Create_WithRefreshToken_KindIsDropbox()
    {
        var fileSystem = DropboxFileSystem.Create("refresh-token", "app-key", "app-secret");

        Assert.Equal("dropbox", fileSystem.Kind);
    }
}
