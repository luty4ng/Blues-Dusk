﻿
namespace GameKit.Download
{
    /// <summary>
    /// 下载代理辅助器完成事件。
    /// </summary>
    public sealed class DownloadAgentHelperCompleteEventArgs : GameKitEventArgs
    {
        /// <summary>
        /// 初始化下载代理辅助器完成事件的新实例。
        /// </summary>
        public DownloadAgentHelperCompleteEventArgs()
        {
            Length = 0L;
        }

        /// <summary>
        /// 获取下载的数据大小。
        /// </summary>
        public long Length
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建下载代理辅助器完成事件。
        /// </summary>
        /// <param name="length">下载的数据大小。</param>
        /// <returns>创建的下载代理辅助器完成事件。</returns>
        public static DownloadAgentHelperCompleteEventArgs Create(long length)
        {
            if (length < 0L)
            {
                throw new GameKitException("Length is invalid.");
            }

            DownloadAgentHelperCompleteEventArgs downloadAgentHelperCompleteEventArgs = ReferencePool.Acquire<DownloadAgentHelperCompleteEventArgs>();
            downloadAgentHelperCompleteEventArgs.Length = length;
            return downloadAgentHelperCompleteEventArgs;
        }

        /// <summary>
        /// 清理下载代理辅助器完成事件。
        /// </summary>
        public override void Clear()
        {
            Length = 0L;
        }
    }
}
