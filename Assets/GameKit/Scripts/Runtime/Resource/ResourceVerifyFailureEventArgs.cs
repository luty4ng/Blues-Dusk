﻿

using GameKit;
using GameKit.Event;

namespace UnityGameKit.Runtime
{
    /// <summary>
    /// 资源校验失败事件。
    /// </summary>
    public sealed class ResourceVerifyFailureEventArgs : GameEventArgs
    {
        /// <summary>
        /// 资源校验失败事件编号。
        /// </summary>
        public static readonly int EventId = typeof(ResourceVerifyFailureEventArgs).GetHashCode();

        /// <summary>
        /// 初始化资源校验失败事件的新实例。
        /// </summary>
        public ResourceVerifyFailureEventArgs()
        {
            Name = null;
        }

        /// <summary>
        /// 获取资源校验失败事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建资源校验失败事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的资源校验失败事件。</returns>
        public static ResourceVerifyFailureEventArgs Create(GameKit.Resource.ResourceVerifyFailureEventArgs e)
        {
            ResourceVerifyFailureEventArgs resourceVerifyFailureEventArgs = ReferencePool.Acquire<ResourceVerifyFailureEventArgs>();
            resourceVerifyFailureEventArgs.Name = e.Name;
            return resourceVerifyFailureEventArgs;
        }

        /// <summary>
        /// 清理资源校验失败事件。
        /// </summary>
        public override void Clear()
        {
            Name = null;
        }
    }
}
