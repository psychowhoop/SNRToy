using UnityEngine;
using YooAsset;
using SNRKWordDefine;
using System.Collections;
using System;

namespace YooAssetHelper
{
    public static class YooHelper
    {
        public static string GetPackageVersion(string packageName = KWord.PkgUnityObj)
        {
            string version = string.Empty;
            var defPackage = YooAssets.TryGetPackage(packageName);
            if (defPackage != null)
            {
                version = defPackage.GetPackageVersion();
            }

            return version;
        }


        public static T GetAssetSync<T>(string resourceName, string packageName = KWord.PkgUnityObj) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return null;
            }

            var package = YooAssets.TryGetPackage(packageName);
            if (package != null)
            {
                var loadData = package.LoadAssetSync<T>(resourceName);
                if (loadData != null && loadData.AssetObject is T)
                {
                    T retData = loadData.AssetObject as T;

                    return retData;
                }

            }

            return null;

        }


        public static IEnumerator GetTexture2DAsync(string texName, Action<object> action, string packageName = KWord.PkgUnityObj)
        {
            var package = YooAssets.TryGetPackage(packageName);
            if (package != null)
            {
                AssetHandle loadData = package.LoadAssetAsync<Texture2D>(texName);
                yield return loadData;

                Texture2D imgTex = loadData.AssetObject as Texture2D;
                action?.Invoke(imgTex);

            }
            yield return null;
        }



    }
}



