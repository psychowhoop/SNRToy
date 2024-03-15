using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SNRKWordDefine;


/// <summary>
/// 下载更新文件
/// </summary>
public class FsmDownloadPackageFiles : IStateNode
{
	private StateMachine _machine;

	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		PatchEventDefine.PatchStatesChange.SendEventMessage("开始下载补丁文件！", _machine);
		var coBehaviour = (MonoBehaviour)_machine.GetBlackboardValue(KWord.CoroutineBehaviour);
		coBehaviour.StartCoroutine(BeginDownload());
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}

	private IEnumerator BeginDownload()
	{
		var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue(KWord.Downloader);
		downloader.OnDownloadErrorCallback = WebFileDownloadFailed;
		downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
		downloader.BeginDownload();
		yield return downloader;

		// 检测下载结果
		if (downloader.Status != EOperationStatus.Succeed)
			yield break;

		_machine.ChangeState<FsmDownloadPackageOver>();
	}

	public void WebFileDownloadFailed(string fileName, string error)
	{
		PatchEventDefine.WebFileDownloadFailed.SendEventMessage(fileName, error, _machine);
	}

	public void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
	{
		PatchEventDefine.DownloadProgressUpdate.SendEventMessage(totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes, _machine);
	}






}