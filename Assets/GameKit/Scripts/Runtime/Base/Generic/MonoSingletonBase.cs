using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameKit;

namespace UnityGameKit.Runtime
{
    public abstract class MonoSingletonBase<T> : MonoBehaviour where T : MonoSingletonBase<T>
    {
        private bool isActive = true;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }

        private static T Current;
        public static T current
        {
            get
            {
                if (Current == null)
                    Debug.LogError($"Mono Singleton Is Not Initialized.");
                return Current;
            }
        }

        protected virtual void Awake()
        {
            if (Current == null)
                Current = this as T;
        }
        public virtual void Enable() => isActive = true;
        public virtual void Disable() => isActive = false;
        public virtual void ShutDown()
        {
            Disable();
            DestroyImmediate(this.gameObject);
        }
        public virtual void Clear() {}
    }
}


