using System.Collections;
using System.Collections.Generic;
using System.IO;
using SNRKWordDefine;
using SNRLogHelper;
using UnityEditor;
using UnityEngine;


/// <summary>
/// not singleton
/// used for add sprite from the choosed folder
/// into sptlist
/// </summary>
[ExecuteInEditMode]
public class SpriteManager : MonoBehaviour
{

    public bool _refreshNow = false;
    public string _sptPath = "";//base is Assets

    public List<Sprite> _sptList = new List<Sprite>();
    public Dictionary<string, Sprite> _sptDic = new Dictionary<string, Sprite>();



    void Awake()
    {
        UpdateDicFromList();
    }

    public void UpdateDicFromList()
    {
        for (int i = 0; i < _sptList.Count; ++i)
        {
            string sptName = _sptList[i].name;
            _sptDic[sptName] = _sptList[i];
        }

    }



    // Start is called before the first frame update
    void Start()
    {

    }




    public Sprite GetSprite(string sptName)
    {
        Sprite spt;
        _sptDic.TryGetValue(sptName, out spt);

        return spt;
    }


    #region Editor

#if UNITY_EDITOR
    List<string> _sptPathList = new List<string>();

    // Update is called once per frame
    void Update()
    {
        if (_refreshNow)
        {
            _refreshNow = false;
            _sptList.Clear();
            _sptDic.Clear();
            _sptPathList.Clear();

            string rootPath = Application.dataPath + _sptPath;
            DirectoryInfo dirInfo = new DirectoryInfo(rootPath);
            GetAllPictureFile(dirInfo);

            for (int i = 0; i < _sptPathList.Count; ++i)
            {
                Sprite spt = AssetDatabase.LoadAssetAtPath<Sprite>(_sptPathList[i]);
                _sptList.Add(spt);
                _sptDic[spt.name] = spt;
            }

            //update content to prefab
            // GameObject prefab = PrefabUtility.GetPrefabInstanceHandle(gameObject) as GameObject;
            // if (prefab != null)
            // {
            //     string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
            //     PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, prefabPath, InteractionMode.AutomatedAction);
            // }
            // else
            // {
            //     SLog.Err("game object is not part of a prefab instance");
            // }
        }
    }


    void GetAllPictureFile(DirectoryInfo dir)
    {
        FileInfo[] fileList = dir.GetFiles();
        foreach (FileInfo file in fileList)
        {
            if (file.Extension == KWord.Dotpng || file.Extension == KWord.Dotjpg)
            {
                string filePath = file.FullName;
                filePath = filePath.Substring(filePath.IndexOf(KWord.Assets));
                _sptPathList.Add(filePath);
            }
        }

        DirectoryInfo[] subDirList = dir.GetDirectories();
        foreach (DirectoryInfo subDir in subDirList)
        {
            GetAllPictureFile(subDir);
        }
    }



#endif


    #endregion


}
