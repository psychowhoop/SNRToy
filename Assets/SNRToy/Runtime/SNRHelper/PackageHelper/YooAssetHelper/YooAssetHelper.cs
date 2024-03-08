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
        public static Texture2D GetTexture2D(string texName, string packageName = KWord.PkgUnityObj)
        {
            var package = YooAssets.TryGetPackage(packageName);
            if (package != null)
            {
                var loadData = package.LoadAssetSync<Texture2D>(texName);
                if (loadData != null && loadData.AssetObject is Texture2D)
                {
                    Texture2D imgTex = loadData.AssetObject as Texture2D;

                    return imgTex;
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



