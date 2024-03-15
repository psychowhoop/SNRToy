using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniFramework.Event;
using SNRHotUpdate;
using SNRLogHelper;
using SNRKWordDefine;
using UniFramework.Machine;


public class PatchWindow : MonoBehaviour
{

    /// <summary>
    /// 对话框封装类
    /// </summary>
    private class MessageBox
    {
        private GameObject _cloneObject;
        private Text _content;
        private Button _btnOK;
        private System.Action _clickOK;



        public bool ActiveSelf
        {
            get
            {
                return _cloneObject.activeSelf;
            }
        }

        public void Create(GameObject cloneObject)
        {
            _cloneObject = cloneObject;
            _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
            _btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
            _btnOK.onClick.AddListener(OnClickYes);
        }
        public void Show(string content, System.Action clickOK)
        {
            _content.text = content;
            _clickOK = clickOK;
            _cloneObject.SetActive(true);
            _cloneObject.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            _content.text = string.Empty;
            _clickOK = null;
            _cloneObject.SetActive(false);
        }
        private void OnClickYes()
        {
            _clickOK?.Invoke();
            Hide();
        }
    }

    public List<PatchOperation> mPatchList = null;

    private readonly EventGroup _eventGroup = new EventGroup();
    private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();
    public Dictionary<StateMachine, SMFileData> mDownloadData = new Dictionary<StateMachine, SMFileData>();

    // UGUI相关
    private GameObject _messageBoxObj;
    private Slider _slider;
    private Text _tips;


    void Awake()
    {
        _slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
        _tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
        _tips.text = "Initializing the game world !";
        _messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
        _messageBoxObj.SetActive(false);

        _eventGroup.AddListener<PatchEventDefine.InitializeFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.PatchStatesChange>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.FoundUpdateFiles>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.DownloadProgressUpdate>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.PackageVersionUpdateFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.PatchManifestUpdateFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<PatchEventDefine.WebFileDownloadFailed>(OnHandleEventMessage);
    }
    void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }


    private StateMachine GetCurShowTipMachine()
    {
        if (mPatchList == null)
        {
            return null;
        }

        for (int i = 0; i < mPatchList.Count; ++i)
        {
            StateMachine machine = mPatchList[i].GetMachine();
            if (machine.IsInState<FsmUpdaterDone>())
            {
                continue;
            }
            return machine;
        }

        return null;
    }

    bool IsMsgMachineInPatchList(IEventMessage msg)
    {
        StateMachine msgMac = msg.pasData as StateMachine;
        foreach (PatchOperation patch in mPatchList)
        {
            StateMachine itemMac = patch.GetMachine();
            if (msgMac == itemMac)
            {
                return true;
            }
        }

        return false;
    }

    string PkgNameFromMsg(IEventMessage message)
    {
        if (message == null)
        {
            return "";
        }

        StateMachine mac = message.pasData as StateMachine;
        return mac.PackageName;
    }

    string GetTipMsgHeader(IEventMessage message)
    {
        if (message == null)
        {
            return "";
        }
        string pkgName = PkgNameFromMsg(message);

        return $"pkg {pkgName} send msg {message.GetType()} ----- ";
    }

    SMFileData GetAllFileData()
    {
        SMFileData retData = new SMFileData(0, 0, 0, 0);
        foreach (SMFileData data in mDownloadData.Values)
        {
            retData.allFileCount += data.allFileCount;
            retData.allFileBytes += data.allFileBytes;
            retData.downFileBytes += data.downFileBytes;
            retData.downFileCount += data.downFileCount;
        }

        return retData;
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        string tipMsgHeader = GetTipMsgHeader(message);

        if (mPatchList == null)
        {
            throw new System.NotImplementedException($"{tipMsgHeader} Patch list cannot be null");
        }

        if (!IsMsgMachineInPatchList(message))
        {
            SLog.Warn($"{tipMsgHeader} patch of machine not in patch list ");
            return;
        }

        if (message is PatchEventDefine.InitializeFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryInitialize.SendEventMessage(message.pasData);
            };
            ShowMessageBox($"{tipMsgHeader} Failed to initialize package !", callback);
        }
        else if (message is PatchEventDefine.PatchStatesChange)
        {
            StateMachine msgMachine = message.pasData as StateMachine;
            StateMachine curShowMachine = GetCurShowTipMachine();
            if (curShowMachine != null && curShowMachine == msgMachine)
            {
                int idx = mPatchList.IndexOf(curShowMachine.Owner as PatchOperation);
                var msg = message as PatchEventDefine.PatchStatesChange;
                string showTxt = $"{tipMsgHeader}" + "patch " + idx + " -- state  " + msg.Tips;
                _tips.text = showTxt;
            }
            else if (curShowMachine == null)
            {
                _tips.text = $"{tipMsgHeader} no cur show patch!!!!! ";
            }
        }
        else if (message is PatchEventDefine.FoundUpdateFiles)
        {
            foreach (PatchOperation patch in mPatchList)
            {
                if (patch.GetMachine().IsCalculateDownFileCountComplete())
                {
                    StateMachine machine = patch.GetMachine();
                    SMFileData fileData = machine.GetProbalyDownloadFileSize();
                    mDownloadData[machine] = fileData;
                }
                else
                {
                    SLog.Log($"{tipMsgHeader} not calculate all download data");
                    return;
                }
            }

            System.Action callback = () =>
            {
                UserEventDefine.UserBeginDownloadWebFiles.SendEventMessage(message.pasData);
            };

            SMFileData allData = GetAllFileData();
            float sizeMB = allData.allFileBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox($"{tipMsgHeader} Found update patch files, Total count {allData.allFileCount} Total szie {totalSizeMB}MB", callback);

        }
        else if (message is PatchEventDefine.DownloadProgressUpdate)
        {
            StateMachine msgMachine = message.pasData as StateMachine;
            SMFileData fData = mDownloadData[msgMachine];
            var msg = message as PatchEventDefine.DownloadProgressUpdate;
            fData.allFileCount = msg.TotalDownloadCount;
            fData.downFileCount = msg.CurrentDownloadCount;
            fData.allFileBytes = msg.TotalDownloadSizeBytes;
            fData.downFileBytes = msg.CurrentDownloadSizeBytes;

            SMFileData allData = GetAllFileData();

            _slider.value = (float)(allData.downFileBytes / allData.allFileBytes);
            string currentSizeMB = (allData.downFileBytes / 1048576f).ToString("f1");
            string totalSizeMB = (allData.allFileBytes / 1048576f).ToString("f1");
            _tips.text = $"{tipMsgHeader} allData {allData.downFileCount}/{allData.allFileCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }
        else if (message is PatchEventDefine.PackageVersionUpdateFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryUpdatePackageVersion.SendEventMessage(message.pasData);
            };
            ShowMessageBox($"{tipMsgHeader} Failed to update static version, please check the network status.", callback);
        }
        else if (message is PatchEventDefine.PatchManifestUpdateFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryUpdatePatchManifest.SendEventMessage(message.pasData);
            };
            ShowMessageBox($"{tipMsgHeader} Failed to update patch manifest, please check the network status.", callback);
        }
        else if (message is PatchEventDefine.WebFileDownloadFailed)
        {
            var msg = message as PatchEventDefine.WebFileDownloadFailed;
            System.Action callback = () =>
            {
                UserEventDefine.UserTryDownloadWebFiles.SendEventMessage(message.pasData);
            };
            ShowMessageBox($"{tipMsgHeader} Failed to download file : {msg.FileName}", callback);
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    private void ShowMessageBox(string content, System.Action ok)
    {
        // 尝试获取一个可用的对话框
        MessageBox msgBox = null;
        for (int i = 0; i < _msgBoxList.Count; i++)
        {
            var item = _msgBoxList[i];
            if (item.ActiveSelf == false)
            {
                msgBox = item;
                break;
            }
        }

        // 如果没有可用的对话框，则创建一个新的对话框
        if (msgBox == null)
        {
            msgBox = new MessageBox();
            var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
            msgBox.Create(cloneObject);
            _msgBoxList.Add(msgBox);
        }

        // 显示对话框
        msgBox.Show(content, ok);
    }
}