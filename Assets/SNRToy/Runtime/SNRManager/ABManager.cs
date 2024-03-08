//not use now

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Reflection;
// using UnityEngine;



// public class ABData
// {
//     public string name;
//     public AssetBundle ab;
//     public int refNum;
// }

// public class ABManager : MonoBehaviour
// {
//     private static ABManager _instance;

//     public Dictionary<string, List<string>> reflist = new Dictionary<string, List<string>>();

//     // #if !UNITY_EDITOR
//     //         assetPath = Application.persistentDataPath + "/AB/";
//     // #else
//     //         assetPath = Application.streamingAssetsPath + "/";
//     // #endif
//     public string assetPath = Application.streamingAssetsPath + "/";
//     public Dictionary<string, byte[]> assetBytes = new Dictionary<string, byte[]>();
//     public Dictionary<string, ABData> abDic = new Dictionary<string, ABData>();


//     public static ABManager Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 GameObject go = new GameObject("ABManager");
//                 _instance = go.AddComponent<ABManager>();
//                 DontDestroyOnLoad(go); // 防止在场景切换时被销毁
//             }
//             return _instance;
//         }
//     }

//     private void OnDestroy()
//     {
//         _instance = null;
//     }


//     public byte[] GetAssetBytes(string dllName)
//     {
//         return assetBytes[dllName];
//     }

//     public AssetBundle LoadAB(string dllName)
//     {
//         return AssetBundle.LoadFromMemory(GetAssetBytes(dllName));
//     }

//     public T Load<T>(string refName, string abName, string assetName) where T : UnityEngine.Object
//     {
//         AssetBundle ab = GetLoadAb(refName, abName, assetName);

//         return ab.LoadAsset<T>(assetName);
//     }


//     private AssetBundle GetLoadAb(string refName, string abName, string assetName)
//     {
//         AssetBundle ab;
//         if (abDic.ContainsKey(abName))
//         {
//             ab = abDic[abName].ab;
//             abDic[abName].refNum++;
//         }
//         else
//         {
//             ab = AssetBundle.LoadFromFile(assetPath + abName);
//             ABData data = new ABData();
//             data.name = abName;
//             data.ab = ab;
//             data.refNum = 1;
//             abDic.Add(abName, data);
//         }

//         if (ab != null)
//         {
//             if (!reflist.ContainsKey(refName))
//             {
//                 reflist.Add(refName, new List<string>());
//             }
//             reflist[refName].Add(abName);
//         }

//         return ab;
//     }

//     public GameObject LoadObj(string abName, string assetName)
//     {
//         AssetBundle ab = GetLoadAb(assetName, abName, assetName);

//         return ab.LoadAsset<GameObject>(assetName);
//     }


//     public Assembly GetAssembly(string assemblyName)
//     {
//         Assembly assembly = Assembly.Load(GetAssetBytes(assemblyName + ".dll.bytes"));

//         return assembly;
//     }

//     //typeName 比如类名  assemblyName不带dll和bytes扩展名
//     public Type GetScriptType(string assemblyName, string typeName)
//     {
//         Assembly asbly = this.GetAssembly(assemblyName);

//         return asbly.GetType(typeName);
//     }


//     public void Release(string refName)
//     {
//         if (reflist.ContainsKey(refName))
//         {
//             foreach (var abName in reflist[refName])
//             {
//                 if (abDic.ContainsKey(abName))
//                 {
//                     abDic[abName].refNum--;
//                     if (abDic[abName].refNum <= 0)
//                     {

//                         ScheduleOnce.Instance.AddSchedule(9, (obj) =>
//                         {
//                             object[] arr = obj as object[];
//                             ABData data = arr[0] as ABData;
//                             if (data.refNum <= 0)
//                             {
//                                 data.ab.Unload(true);
//                                 abDic.Remove(abName);
//                                 Debug.Log(data.name + "释放成功");
//                             }
//                         }, abDic[abName]);

//                     }
//                 }
//             }
//             reflist.Remove(refName);
//         }
//     }
// }











// public class ScheduleOnce : MonoBehaviour
// {
//     private static ScheduleOnce _instance;

//     public static ScheduleOnce Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 GameObject go = new GameObject("ScheduleOnce");
//                 _instance = go.AddComponent<ScheduleOnce>();
//                 DontDestroyOnLoad(go); // 防止在场景切换时被销毁
//             }
//             return _instance;
//         }
//     }

//     private void OnDestroy()
//     {
//         _instance = null;
//     }

//     // 添加延迟执行的方法
//     public void AddSchedule(float delayInSeconds, Action<object> action, object param)
//     {
//         StartCoroutine(DelayedActionCoroutine(delayInSeconds, action, param));
//     }

//     private IEnumerator DelayedActionCoroutine(float delayInSeconds, Action<object> action, object param)
//     {
//         yield return new WaitForSeconds(delayInSeconds);
//         action?.Invoke(param);
//     }
// }




