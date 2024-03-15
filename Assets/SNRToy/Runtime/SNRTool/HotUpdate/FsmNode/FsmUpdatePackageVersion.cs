using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SNRKWordDefine;


/// <summary>
/// 更新资源版本号
/// </summary>
public class FsmUpdatePackageVersion : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("获取最新的资源版本 !", _machine);
        var coBehaviour = (MonoBehaviour)_machine.GetBlackboardValue(KWord.CoroutineBehaviour);
        coBehaviour.StartCoroutine(UpdatePackageVersion());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator UpdatePackageVersion()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var packageName = (string)_machine.GetBlackboardValue(KWord.PackageName);
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            //heywait 比如当version.json文件里的版本在cdn里找不到时 会到这
            //加个统一开关 可以线上调试log类似于显示logtoScreen
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage(_machine);
        }
        else
        {
            _machine.SetBlackboardValue(KWord.PackageVersion, operation.PackageVersion);
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
    }
}