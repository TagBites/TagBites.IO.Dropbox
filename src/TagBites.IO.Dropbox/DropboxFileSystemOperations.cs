using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using TagBites.IO.Operations;

namespace TagBites.IO.Dropbox
{
    internal class DropboxFileSystemOperations : IFileSystemAsyncWriteOperations, IFileSystemMetadataSupport, IDisposable
    {
        private const string RootDirectory = "/";
        private readonly DropboxClient _dropboxClient;

        public string Kind => "dropbox";
        public string? Name => null;

        #region IFileSystemOperationsMetadataSupport

        bool IFileSystemMetadataSupport.SupportsIsHiddenMetadata => false;
        bool IFileSystemMetadataSupport.SupportsIsReadOnlyMetadata => false;
        bool IFileSystemMetadataSupport.SupportsLastWriteTimeMetadata => false;

        #endregion

        public DropboxFileSystemOperations(string oauth2Token)
        {
            DropboxCertHelper.InitializeCertPinning();
            _dropboxClient = new DropboxClient(oauth2Token);
        }
        public DropboxFileSystemOperations(string oauth2RefreshToken, string appKey, string appSecret)
        {
            DropboxCertHelper.InitializeCertPinning();
            _dropboxClient = new DropboxClient(oauth2RefreshToken, appKey, appSecret);
        }



        public async Task<IFileSystemStructureLinkInfo> GetLinkInfoAsync(string fullName)
        {
            Guard.ArgumentNotNullOrEmpty(fullName, nameof(fullName));

            if (fullName == RootDirectory)
                return new RootDirectoryInfo();

            try
            {
                var metadata = await _dropboxClient.Files.GetMetadataAsync(fullName).ConfigureAwait(false);

                return GetInfo(metadata);
            }
            catch (ApiException<GetMetadataError> e)
            {
                return null;
            }
        }
        public string CorrectPath(string path)
        {
            return path switch
            {
                null => null,
                RootDirectory => path,
                _ => path.StartsWith("/") ? path : "/" + path
            };
        }

        #region Files

        public async Task ReadFileAsync(FileLink file, Stream stream)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            var argument = new DownloadArg(file.FullName);
            var result = await _dropboxClient.Files.DownloadAsync(argument).ConfigureAwait(false);
            using var rs = await result.GetContentAsStreamAsync().ConfigureAwait(false);
            await rs.CopyToAsync(stream).ConfigureAwait(false);
        }
        public async Task<IFileLinkInfo> WriteFileAsync(FileLink file, Stream stream, bool overwrite)
        {
            Guard.ArgumentNotNull(file, nameof(file));
            Guard.ArgumentNotNull(stream, nameof(stream));

            var arguments = new UploadArg(file.FullName, file.Exists ? (WriteMode)WriteMode.Overwrite.Instance : WriteMode.Add.Instance);
            var result = await _dropboxClient.Files.UploadAsync(arguments, stream).ConfigureAwait(false);
            return GetFileInfo(result);
        }
        public async Task<IFileLinkInfo> MoveFileAsync(FileLink source, FileLink destination, bool overwrite)
        {
            Guard.ArgumentNotNull(source, nameof(source));
            Guard.ArgumentNotNull(destination, nameof(destination));

            var arguments = new RelocationArg(source.FullName, destination.FullName);
            var result = await _dropboxClient.Files.MoveV2Async(arguments).ConfigureAwait(false);

            return GetFileInfo(result.Metadata.AsFile);
        }
        public async Task DeleteFileAsync(FileLink file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            var arguments = new DeleteArg(file.FullName);
            await _dropboxClient.Files.DeleteV2Async(arguments).ConfigureAwait(false);
        }

        #endregion

        #region Directories

        public async Task<IFileSystemStructureLinkInfo> CreateDirectoryAsync(DirectoryLink directory)
        {
            Guard.ArgumentNotNull(directory, nameof(directory));

            var folderArg = new CreateFolderArg(directory.FullName);
            try
            {
                var folder = await _dropboxClient.Files.CreateFolderV2Async(folderArg).ConfigureAwait(false);
                return GetDirectoryInfo(folder.Metadata);
            }
            catch (ApiException<CreateFolderError> e)
            {
                return null;
            }
        }
        public async Task<IFileSystemStructureLinkInfo> MoveDirectoryAsync(DirectoryLink source, DirectoryLink destination)
        {
            Guard.ArgumentNotNull(source, nameof(source));
            Guard.ArgumentNotNull(destination, nameof(destination));

            var arguments = new RelocationArg(source.FullName, destination.FullName);
            var result = await _dropboxClient.Files.MoveV2Async(arguments).ConfigureAwait(false);

            return GetDirectoryInfo(result.Metadata.AsFolder);
        }
        public async Task DeleteDirectoryAsync(DirectoryLink directory, bool recursive)
        {
            Guard.ArgumentNotNull(directory, nameof(directory));

            if (!recursive)
            {
                var items = await _dropboxClient.Files.ListFolderAsync(new ListFolderArg(directory.FullName)).ConfigureAwait(false);
                if (items.Entries.Count > 0)
                    throw new IOException("The directory is not empty.");
            }

            var arg = new DeleteArg(directory.FullName);
            var result = await _dropboxClient.Files.DeleteV2Async(arg).ConfigureAwait(false);
        }

        public async Task<IList<IFileSystemStructureLinkInfo>> GetLinksAsync(DirectoryLink directory, FileSystem.ListingOptions options)
        {
            Guard.ArgumentNotNull(directory, nameof(directory));
            Guard.ArgumentNotNull(options, nameof(options));

            var links = new List<IFileSystemStructureLinkInfo>();

            //if (options.HasSearchPattern)
            //{
            //    options.SearchPatternHandled = true;
            //    var searchOptions = new SearchOptions(
            //        path: directory.FullName,
            //        maxResults: 100UL,
            //        fileStatus: null,
            //        filenameOnly: options.SearchForFiles && !options.SearchForDirectories);
            //    var arguments = new SearchV2Arg(options.SearchPattern, searchOptions);
            //    var result = await _dropboxClient.Files.SearchV2Async(arguments); // TODO

            //    foreach (var match in result.Matches)
            //    {
            //        var metadata = match.Metadata.AsMetadata?.Value;
            //        if (metadata == null)
            //            continue;
            //        links.Add(GetInfo(metadata));
            //    }
            //}
            //else
            {
                // TODO continue list
                options.RecursiveHandled = true;
                var result1 = await _dropboxClient.Files.ListFolderAsync(new ListFolderArg(directory.FullName, recursive: options.Recursive)).ConfigureAwait(false);
                foreach (var metadata in result1.Entries)
                {
                    // Ignore yourself
                    if (metadata.PathDisplay == directory.FullName)
                        continue;

                    if (options.SearchForFiles && metadata.IsFile)
                        links.Add(GetFileInfo(metadata.AsFile));
                    else if (options.SearchForDirectories && metadata.IsFolder)
                        links.Add(GetDirectoryInfo(metadata.AsFolder));
                }
            }

            return links;
        }

        #endregion

        #region Metadata

        public Task<IFileSystemStructureLinkInfo> UpdateMetadataAsync(FileSystemStructureLink link, IFileSystemLinkMetadata metadata)
        {
            Guard.ArgumentNotNull(link, nameof(link));
            Guard.ArgumentNotNull(metadata, nameof(metadata));

            return GetLinkInfoAsync(link.FullName);
        }

        private static IFileSystemStructureLinkInfo GetInfo(Metadata metadata)
        {
            if (metadata == null)
                return null;

            if (metadata.IsFolder)
                return new DirectoryInfo(metadata.AsFolder);

            if (metadata.IsFile)
                return new FileInfo(metadata.AsFile);

            return null;
        }
        private static DirectoryInfo GetDirectoryInfo(FolderMetadata metadata)
        {
            return new DirectoryInfo(metadata);
        }
        private static FileInfo GetFileInfo(FileMetadata metadata)
        {
            return new FileInfo(metadata);
        }

        private static Metadata ToMetadata(FileSystemStructureLink link)
        {
            return link is FileLink f
                ? (Metadata)ToMetadata(f)
                : ToMetadata((DirectoryLink)link);
        }
        private static FileMetadata ToMetadata(FileLink link) => (link.Info as FileInfo)?.Metadata;
        private static FolderMetadata ToMetadata(DirectoryLink link) => (link.Info as DirectoryInfo)?.Metadata;

        #endregion

        public void Dispose() => _dropboxClient?.Dispose();

        private class FileInfo : IFileLinkInfo
        {
            public FileMetadata Metadata { get; }

            public string FullName => Metadata.PathDisplay;
            public bool Exists => !Metadata.IsDeleted;
            public bool? IsDirectory => false;
            public DateTime? CreationTime => null;
            public DateTime? LastWriteTime => Metadata.ClientModified;
            public bool IsHidden => false;
            public bool IsReadOnly => Metadata.FileLockInfo?.IsLockholder == true;

            public string ContentPath => Metadata.PathDisplay;
            public FileHash Hash { get; }
            public long Length => (long)Metadata.Size;

            public FileInfo(FileMetadata metadata)
            {
                Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));

                Hash = new FileHash(FileHashAlgorithm.Md5, metadata.ContentHash); // TODO check
            }
        }
        private class DirectoryInfo : IFileSystemStructureLinkInfo
        {
            public FolderMetadata Metadata { get; }

            public string FullName => Metadata.PathDisplay;
            public bool Exists => !Metadata.IsDeleted;
            public bool? IsDirectory => true;
            public DateTime? CreationTime => null;
            public DateTime? LastWriteTime => null;
            public bool IsHidden => false;
            public bool IsReadOnly => false;

            public DirectoryInfo(FolderMetadata metadata)
            {
                Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            }
        }
        public class RootDirectoryInfo : IFileSystemStructureLinkInfo
        {
            public string FullName => "/";
            public bool Exists => true;
            public bool? IsDirectory => true;

            public DateTime? CreationTime => null;
            public DateTime? LastWriteTime => null;

            public bool IsHidden => false;
            public bool IsReadOnly => false;
        }
    }
}
