﻿

using GameKit.FileSystem;

namespace UnityGameKit.Editor.ResourceTools
{
    public sealed partial class ResourceBuilderController
    {
        private sealed class FileSystemHelper : IFileSystemHelper
        {
            public FileSystemStream CreateFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
            {
                return new CommonFileSystemStream(fullPath, access, createNew);
            }
        }
    }
}
