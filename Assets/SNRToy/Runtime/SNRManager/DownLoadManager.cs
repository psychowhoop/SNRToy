using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SNRLogHelper;


namespace SNRDownloadManager
{
    public class DownloadManager : MonoBehaviour
    {


        //     downloadManager = gameObject.AddComponent<DownloadManager>();
        //         string fileUrl = "http://127.0.0.1/CDN/Golden/version.json";

        //     // 调用DownloadManager开始下载，并传入匿名函数作为回调
        //     downloadManager.StartWithCallBack(fileUrl, (success, fileContent) =>
        //         {
        //             if (success)
        //             {
        //                 Debug.Log("Download successful! File content: " + fileContent);
        //                 // 在这里可以继续处理下载成功后的逻辑
        //             }
        //             else
        // {
        //     Debug.LogError("Download failed!");
        //     // 在这里可以处理下载失败后的逻辑
        // }
        //         });





        public void StartWithCallBack(string url, System.Action<bool, string> callback)
        {
            StartCoroutine(DownloadFileCB(url, callback));
        }

        IEnumerator DownloadFileCB(string url, System.Action<bool, string> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                SLog.Err("Failed to download file: " + www.error);
                // 调用回调函数，传递下载失败的信息
                callback?.Invoke(false, null);
            }
            else
            {
                // 文件下载成功，可以在这里读取文件内容
                string fileContent = www.downloadHandler.text;
                SLog.Log("File content: " + fileContent);

                // 调用回调函数，并传递文件内容和下载成功的标志
                callback?.Invoke(true, fileContent);
            }
        }


        // handle style
        public class DownloadHandle
        {
            public bool success;
            public string data;

            public DownloadHandle(bool success, string data)
            {
                this.success = success;
                this.data = data;
            }
        }

        // 调用下载管理器的方法，并等待其执行完毕
        // var coroutine = downloadManager.StartWithHandle(url);
        // yield return coroutine;

        //     // 获取下载处理结果   系统有个自带的 DownloadHandler 所以带上downloadmanager
        //     DownloadManager.DownloadHandle downloadHandle = coroutine.Current as DownloadManager.DownloadHandle;

        //         // 处理下载结果
        //         if (downloadHandle.success)
        //         {
        //             Debug.Log("Download successful! File content: " + downloadHandle.data);
        //             // 在这里可以继续处理下载成功后的逻辑
        //         }
        //         else
        // {
        //     Debug.LogError("Download failed!");
        //     // 在这里可以处理下载失败后的逻辑
        // }
        //     

        public IEnumerator StartWithHandle(string url)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                SLog.Err("Failed to download file: " + www.error);
                yield return new DownloadHandle(false, null);
            }
            else
            {
                string fileContent = www.downloadHandler.text;
                SLog.Log("File content: " + fileContent);
                yield return new DownloadHandle(true, fileContent);
            }
        }

    }


}






