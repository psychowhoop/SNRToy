using System.Collections;
using System.Collections.Generic;
using SNRHotUpdate;
using UnityEngine;
using YooAsset;

public class HotController
{

    public IEnumerator HotUpdatePatchList(List<PatchOperation> patchList, System.Action onAllPatchComplete, PatchWindow patchWnd)
    {
        if (patchWnd != null)
        {
            patchWnd.mPatchList = patchList;
        }

        int completedPatch = 0;

        // 启动所有的操作
        foreach (var patch in patchList)
        {
            YooAssets.StartOperation(patch);
        }

        // 等待所有的操作完成
        while (completedPatch < patchList.Count)
        {
            for (int i = 0; i < patchList.Count; i++)
            {
                if (patchList[i].CurStep != PatchOperation.ESteps.Done)
                {
                    // 如果某个操作未完成，等待下一帧
                    yield return null;
                }
                else
                {
                    // 如果某个操作完成，增加计数器
                    completedPatch++;
                }
            }
        }

        // 所有操作完成后调用回调函数
        onAllPatchComplete?.Invoke();
    }





}
