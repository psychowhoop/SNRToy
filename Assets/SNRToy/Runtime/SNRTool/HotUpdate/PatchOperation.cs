using UnityEngine;
using UniFramework.Machine;
using UniFramework.Event;
using YooAsset;
using SNRKWordDefine;
using System.Collections;
using UnityEngine.Networking;
using SNRDownloadManager;
using SNRLogHelper;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;



namespace SNRHotUpdate
{
    public class PatchOperation : GameAsyncOperation
    {

        public class PatchModel
        {
            public string pkgName;
            public EDefaultBuildPipeline buildPipeline;
            public EPlayMode playMode;
            public string serAddress;
            public string cdnProjFolder;
            public MonoBehaviour coroutineBhv;
        }


        public enum ESteps
        {
            None,
            Update,
            Done,
        }

        public static Dictionary<string, string> LastVerDic = null;
        private readonly EventGroup _eventGroup = new EventGroup();
        private readonly StateMachine _machine;
        private ESteps _steps = ESteps.None;
        public ESteps CurStep
        {
            get
            {
                return _steps;
            }
        }

        public StateMachine GetMachine()
        {
            return _machine;
        }

        public PatchOperation(PatchModel pModel) : this((pModel.pkgName, pModel.buildPipeline.ToString(), pModel.playMode, pModel.serAddress, pModel.cdnProjFolder, pModel.coroutineBhv))
        {

        }

        private PatchOperation((
            string pkgName,
            string buildPipeline,
            EPlayMode playMode,
            string serAddress,
            string cdnProjFolder,
            MonoBehaviour coroutineBhv
        ) initData)
        {
            // 注册监听事件
            _eventGroup.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);

            // 创建状态机
            _machine = new StateMachine(this);
            _machine.AddNode<FsmInitializePackage>();
            _machine.AddNode<FsmUpdatePackageVersion>();
            _machine.AddNode<FsmUpdatePackageManifest>();
            _machine.AddNode<FsmCreatePackageDownloader>();
            _machine.AddNode<FsmDownloadPackageFiles>();
            _machine.AddNode<FsmDownloadPackageOver>();
            _machine.AddNode<FsmClearPackageCache>();
            _machine.AddNode<FsmUpdaterDone>();

            _machine.SetBlackboardValue(KWord.PackageName, initData.pkgName);
            _machine.SetBlackboardValue(KWord.PlayMode, initData.playMode);
            _machine.SetBlackboardValue(KWord.BuildPipeline, initData.buildPipeline);
            _machine.SetBlackboardValue(KWord.ServerAddress, initData.serAddress);
            _machine.SetBlackboardValue(KWord.CDNProjectFolder, initData.cdnProjFolder);
            _machine.SetBlackboardValue(KWord.CoroutineBehaviour, initData.coroutineBhv);
        }
        protected override void OnStart()
        {
            MonoBehaviour bhv = (MonoBehaviour)_machine.GetBlackboardValue(KWord.CoroutineBehaviour);
            bhv.StartCoroutine(DownloadLastVersionFileAndInitPkg());
        }

        IEnumerator DownloadLastVersionFileAndInitPkg()
        {
            IEnumerator coroutine = null;
            if (PatchOperation.LastVerDic == null)
            {
                string serAddress = (string)_machine.GetBlackboardValue(KWord.ServerAddress);
                string cdnFolderName = (string)_machine.GetBlackboardValue(KWord.CDNProjectFolder);
                string versionFileUrl = $"{serAddress}/CDN/{cdnFolderName}/version.json";
                MonoBehaviour bhv = (MonoBehaviour)_machine.GetBlackboardValue(KWord.CoroutineBehaviour);
                DownloadManager dm = bhv.gameObject.AddComponent<DownloadManager>();
                coroutine = dm.StartWithHandle(versionFileUrl);
                yield return coroutine;

                DownloadManager.DownloadHandle handle = coroutine.Current as DownloadManager.DownloadHandle;
                if (handle.success)
                {
                    var verDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.data);
                    PatchOperation.LastVerDic = verDic;
                    SLog.Log("down version succeed " + verDic.ToString());
                }
                else
                {
                    SLog.Err("down load version file failed " + versionFileUrl);
                }
            }

            yield return coroutine;

            _machine.SetBlackboardValue(KWord.LastVersionDic, PatchOperation.LastVerDic);
            _steps = ESteps.Update;
            _machine.Run<FsmInitializePackage>();
        }



        protected override void OnUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.Update)
            {
                _machine.Update();
                if (_machine.CurrentNode == typeof(FsmUpdaterDone).FullName)
                {
                    _eventGroup.RemoveAllListener();
                    Status = EOperationStatus.Succeed;
                    _steps = ESteps.Done;
                }
            }
        }
        protected override void OnAbort()
        {
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        private void OnHandleEventMessage(IEventMessage message)
        {

            if (message.pasData == null)
            {
                throw new System.NotImplementedException($"{message.GetType()} not pass the machine");
            }

            if (message.pasData != _machine)
            {
                SLog.Log("not my machine!");
                return;
            }

            if (message is UserEventDefine.UserTryInitialize)
            {
                _machine.ChangeState<FsmInitializePackage>();
            }
            else if (message is UserEventDefine.UserBeginDownloadWebFiles)
            {
                _machine.ChangeState<FsmDownloadPackageFiles>();
            }
            else if (message is UserEventDefine.UserTryUpdatePackageVersion)
            {
                _machine.ChangeState<FsmUpdatePackageVersion>();
            }
            else if (message is UserEventDefine.UserTryUpdatePatchManifest)
            {
                _machine.ChangeState<FsmUpdatePackageManifest>();
            }
            else if (message is UserEventDefine.UserTryDownloadWebFiles)
            {
                _machine.ChangeState<FsmCreatePackageDownloader>();
            }
            else
            {
                throw new System.NotImplementedException($"{message.GetType()}");
            }
        }
    }
}
