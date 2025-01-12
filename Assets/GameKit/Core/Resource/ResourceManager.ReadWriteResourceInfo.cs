﻿

using System.Runtime.InteropServices;

namespace GameKit.Resource
{
    internal sealed partial class ResourceManager : GameKitModule, IResourceManager
    {
        [StructLayout(LayoutKind.Auto)]
        private struct ReadWriteResourceInfo
        {
            private readonly string m_FileSystemName;
            private readonly LoadType m_LoadType;
            private readonly int m_Length;
            private readonly int m_HashCode;

            public ReadWriteResourceInfo(string fileSystemName, LoadType loadType, int length, int hashCode)
            {
                m_FileSystemName = fileSystemName;
                m_LoadType = loadType;
                m_Length = length;
                m_HashCode = hashCode;
            }

            public bool UseFileSystem
            {
                get
                {
                    return !string.IsNullOrEmpty(m_FileSystemName);
                }
            }

            public string FileSystemName
            {
                get
                {
                    return m_FileSystemName;
                }
            }

            public LoadType LoadType
            {
                get
                {
                    return m_LoadType;
                }
            }

            public int Length
            {
                get
                {
                    return m_Length;
                }
            }

            public int HashCode
            {
                get
                {
                    return m_HashCode;
                }
            }
        }
    }
}
