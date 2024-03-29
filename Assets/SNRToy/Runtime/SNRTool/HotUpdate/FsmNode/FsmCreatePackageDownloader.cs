﻿using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SNRKWordDefine;


/// <summary>
/// 创建文件下载器
/// </summary>
public class FsmCreatePackageDownloader : IStateNode
{
    public int mFileCount = 0;
    public long mFileBytes = 0;

    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("创建补丁下载器！", _machine);
        var coBehaviour = (MonoBehaviour)_machine.GetBlackboardValue(KWord.CoroutineBehaviour);
        coBehaviour.StartCoroutine(CreateDownloader());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    IEnumerator CreateDownloader()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var packageName = (string)_machine.GetBlackboardValue(KWord.PackageName);
        var package = YooAssets.GetPackage(packageName);
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        _machine.SetBlackboardValue(KWord.Downloader, downloader);

        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("Not found any download files !");
            _machine.ChangeState<FsmUpdaterDone>();
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足 //heywait
            //需要下载的文件总数和总大小
            mFileCount = downloader.TotalDownloadCount;
            mFileBytes = downloader.TotalDownloadBytes;

            PatchEventDefine.FoundUpdateFiles.SendEventMessage(mFileCount, mFileBytes, _machine);
        }
    }

}