﻿

namespace GameKit.Resource
{
    /// <summary>
    /// 单机模式版本资源列表序列化器。
    /// </summary>
    public sealed class PackageVersionListSerializer : GameKitSerializer<PackageVersionList>
    {
        private static readonly byte[] Header = new byte[] { (byte)'G', (byte)'F', (byte)'P' };

        /// <summary>
        /// 初始化单机模式版本资源列表序列化器的新实例。
        /// </summary>
        public PackageVersionListSerializer()
        {
        }

        /// <summary>
        /// 获取单机模式版本资源列表头标识。
        /// </summary>
        /// <returns>单机模式版本资源列表头标识。</returns>
        protected override byte[] GetHeader()
        {
            return Header;
        }
    }
}
