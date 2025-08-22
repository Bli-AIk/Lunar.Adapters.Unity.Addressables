using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunar.Interfaces;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Lunar.Adapters.Unity.Addressables
{
    public class AddressablesAdapter : IResources, IResourcesAsync
    {
        [Obsolete("Use LoadAsync instead.")]
        public T Load<T>(string path)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAsset");
            return UnityEngine.AddressableAssets.Addressables.LoadAsset<T>(path).WaitForCompletion();
        }

        [Obsolete("Use LoadAllAsync instead.")]
        public IEnumerable<T> LoadAll<T>(string path)
        {
            return LoadAll<T>(path, null);
        }

        [Obsolete("Use LoadAllAsync instead.")]
        public IEnumerable<T> LoadAll<T>(IEnumerable<string> paths)
        {
            return LoadAll<T>(paths, null);
        }

        public void Release<T>(T resource)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.Release");
            if (resource is Object obj)
            {
                UnityEngine.AddressableAssets.Addressables.Release(obj);
            }
        }

        public Task<T> LoadAsync<T>(string path, CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetAsync");
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(path);
            return HandleAsyncOperation(handle, path, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(paths, null, UnityEngine.AddressableAssets.Addressables.MergeMode.None, ct);
        }

        public async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(path, null, UnityEngine.AddressableAssets.Addressables.MergeMode.None, ct);
        }


        public static async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            Action<T> callback,
            UnityEngine.AddressableAssets.Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetsAsync");
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync(path, callback, mergeMode);
            return await HandleAsyncOperation(handle, path, ct);
        }

        public static async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            UnityEngine.AddressableAssets.Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(path, null, mergeMode, ct);
        }

        public static async Task<IEnumerable<T>> LoadAllAsync<T>(string path,
            Action<T> callback,
            CancellationToken ct = new())
        {
            return await LoadAllAsync(path, callback, UnityEngine.AddressableAssets.Addressables.MergeMode.None, ct);
        }


        public static async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            UnityEngine.AddressableAssets.Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            return await LoadAllAsync<T>(paths, null, mergeMode, ct);
        }

        public static async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            Action<T> callback,
            CancellationToken ct = new())
        {
            return await LoadAllAsync(paths, callback, UnityEngine.AddressableAssets.Addressables.MergeMode.None, ct);
        }

        public static async Task<IEnumerable<T>> LoadAllAsync<T>(IEnumerable<string> paths,
            Action<T> callback,
            UnityEngine.AddressableAssets.Addressables.MergeMode mergeMode,
            CancellationToken ct = new())
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssetsAsync");
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync(paths, callback, mergeMode);
            return await HandleAsyncOperation(handle, paths, ct);
        }

        /// <summary>
        ///     A private helper method to convert an AsyncOperationHandle into a Task.
        ///     This encapsulates the logic for handling completion, cancellation, and exceptions.
        /// </summary>
        /// <param name="handle">The Addressables async operation handle.</param>
        /// <param name="key">The key or path used for loading, for exception messages.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <typeparam name="TResult">The result type of the operation.</typeparam>
        /// <returns>A Task representing the async operation.</returns>
        private static Task<TResult> HandleAsyncOperation<TResult>(AsyncOperationHandle<TResult> handle,
            object key,
            CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);

            CancellationTokenRegistration ctr = default;
            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() =>
                {
                    UnityEngine.AddressableAssets.Addressables.Release(handle);
                    tcs.TrySetCanceled(ct);
                });
            }

            handle.Completed += h =>
            {
                try
                {
                    if (h.Status == AsyncOperationStatus.Succeeded)
                    {
                        tcs.TrySetResult(h.Result);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception($"Failed to load asset with key: {key}. Status: {h.Status}",
                            h.OperationException));
                    }
                }
                finally
                {
                    ctr.Dispose();
                }
            };

            return tcs.Task;
        }


        [Obsolete("Use LoadAllAsync instead.")]
        public static IEnumerable<T> LoadAll<T>(string path, Action<T> callback)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return UnityEngine.AddressableAssets.Addressables.LoadAssets(path, callback).WaitForCompletion();
        }

        [Obsolete("Use LoadAllAsync instead.")]
        public static IEnumerable<T> LoadAll<T>(IEnumerable<string> paths, Action<T> callback)
        {
            ResourcesUtility.ValidateUnityObjectType<T>("Addressables.LoadAssets");
            return UnityEngine.AddressableAssets.Addressables.LoadAssets(paths, callback).WaitForCompletion();
        }
    }
}