﻿

using System;
using UnityEditor;

namespace UnityGameKit.Editor.ResourceTools
{
    /// <summary>
    /// 资源。
    /// </summary>
    public sealed class Asset : IComparable<Asset>
    {
        private Asset(string guid, Resource resource)
        {
            Guid = guid;
            Resource = resource;
        }

        public string Guid
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return AssetDatabase.GUIDToAssetPath(Guid);
            }
        }

        public Resource Resource
        {
            get;
            set;
        }

        public int CompareTo(Asset asset)
        {
            return string.Compare(Guid, asset.Guid, StringComparison.Ordinal);
        }

        public static Asset Create(string guid)
        {
            return new Asset(guid, null);
        }

        public static Asset Create(string guid, Resource resource)
        {
            return new Asset(guid, resource);
        }
    }
}
