using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if PACKAGE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
#endif
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace GameKit
{
    public class AddressableManager : SingletonBase<AddressableManager>
    {
#if PACKAGE_ADDRESSABLES
        private List<AssetReference> assetReferences;
        private Dictionary<AssetReference, List<GameObject>> gameobjs;

        IEnumerator GetAsynProcess<T>(string keyName, UnityAction<T> callback) where T : Object
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(keyName);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (handle.Result is GameObject)
                    callback?.Invoke(GameObject.Instantiate(handle.Result as GameObject) as T);
                else
                    callback?.Invoke(handle.Result as T);
            }

        }

        IEnumerator GetAsynProcessByLabel<T>(IList<string> labels, UnityAction<T> eachCall, UnityAction<IList<T>> callback) where T : Object
        {
            AsyncOperationHandle<IList<T>> handle =
                Addressables.LoadAssetsAsync<T>(labels,
                    obj =>
                    {
                        eachCall?.Invoke(obj as T);
                    }, Addressables.MergeMode.Union, false);

            yield return handle;
            callback?.Invoke(handle.Result as IList<T>);
        }
#endif

        public void GetAssetAsyn<T>(string keyName, UnityAction<T> callback = null) where T : Object
        {
#if PACKAGE_ADDRESSABLES
            MonoManager.instance.StartCoroutine(GetAsynProcess<T>(keyName, callback));
#endif
        }

        public void GetAssetsAsyn<T>(IList<string> labels, UnityAction<T> eachCall = null, UnityAction<IList<T>> callback = null) where T : Object
        {
#if PACKAGE_ADDRESSABLES
            MonoManager.instance.StartCoroutine(GetAsynProcessByLabel<T>(labels, eachCall, callback));
#endif
        }
    }

}