using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HybridCLR.Editor;
using System.IO;
using System.Linq;
using HybridCLR.Editor.Settings;
using SNRLogHelper;
using System;
using UnityEditorInternal;
using dnlib.DotNet;
using HybridCLR.Editor.Commands;

public class HybridDealDll : Editor
{
    //static string hotDllpath = Application.dataPath + "/../" + HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir;

    //not use pack assetBundle in hybridclr
    //static string abOutPath = $"{Application.dataPath}/AllToHot/HotData/Asset";
    static string codeOutPath = $"{Application.dataPath}/AllToHot/HotCode";

    public static List<string> AOTMetaAssemblyNames = HybridCLRSettings.Instance.patchAOTAssemblies.ToList();


    [MenuItem("HybridCLR/SNRHybridOneClick")]
    public static void SNRHybridOneClick()
    {
        PrebuildCommand.GenerateAll();
        SLog.Log("generate all finished!!!!!!!!!!!!!");

        //注册一个延迟调用，在下一帧执行 AddBytesExtAndMoveFirst 方法
        EditorApplication.delayCall += AddBytesExtAndMoveFirst;

    }

    private static void AddBytesExtAndMoveFirst()
    {
        SLog.Log("in AddBytesExtAndMoveFirst context");
        // 取消注册的延迟调用，确保只在第一次调用 AddBytesExtAndMoveFirst 方法时执行 AddBytesExtAndMove 方法
        EditorApplication.delayCall -= AddBytesExtAndMoveFirst;

        SLog.Log("will do AddBytesExtAndMove !!!!!!!!!");
        // 执行第二个菜单项的逻辑
        AddBytesExtAndMove();
    }


    //[MenuItem("HybridCLR/MoveDllToHot")]

    public static void AddBytesExtAndMove()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        //找到对应目录
        //DirectoryInfo direction = new DirectoryInfo(hotDllpath + "/" + target);
        //获取目录下所有文件
        //FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        AssemblyDefinitionAsset[] assDefAry = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions;

        // 使用 LINQ 的 Select 方法从数组中选择每个对象的 name 成员，并放入一个新的列表中
        List<string> hotDllList = assDefAry.Select(obj => obj.name + ".dll").ToList();
        string hotDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        string hotDllDstDir = codeOutPath;//Application.streamingAssetsPath;

        foreach (var dll in hotDllList)
        {
            string srcDllPath = $"{hotDllSrcDir}/{dll}";
            if (!File.Exists(srcDllPath))
            {
                SLog.Err($"hotDll添加bytes并移动时发生错误:{srcDllPath} ,文件不存在。");
                continue;
            }
            string dllBytesPath = $"{hotDllDstDir}/{dll}.bytes";
            File.Copy(srcDllPath, dllBytesPath, true);
            SLog.Log($"热更dll移动成功 {srcDllPath} -> {dllBytesPath}");
        }

        //把所有dll文件拷贝到destination目录下
        // foreach (var item in files)
        // {
        //     bool isDllFile = item.Name.EndsWith(".dll");
        //     string fileName = Path.GetFileNameWithoutExtension(item.Name);
        //     bool isInHotList = hotDllList.Contains(fileName);

        //     if (isDllFile && isInHotList)
        //     {
        //         string dllPath = $"{hotDllpath + "/" + target}/{item.Name}";
        //         //string dllBytesPath = $"{Application.streamingAssetsPath}/{item.Name}.bytes";
        //         string dllBytesPath = $"{codeOutPath}/{item.Name}.bytes";
        //         File.Copy(dllPath, dllBytesPath, true);
        //         SLog.Log($"hotfix dll {dllPath} -> {dllBytesPath}");

        //     }
        // }
        /*
        List<AssetBundleBuild> abList = new List<AssetBundleBuild>();
        //base打包
        PackDirToABundle(abList, $"{Application.dataPath}/AllToHot/Base/Asset");

        //添加game文件夹下子游戏的打包
        string gamePath = $"{Application.dataPath}/AllToHot/Game/";
        // 获取当前文件夹下的所有子文件夹
        string[] subfolders = Directory.GetDirectories(gamePath);
        // 输出子文件夹数量
        SLog.Log($"Number of subfolders in {gamePath}: {subfolders.Length}");

        // 打包子游戏
        foreach (string subfolder in subfolders)
        {
            string subAssetPath = subfolder + "/Asset";
            PackDirToABundle(abList, subAssetPath);
        }

        //AddABPath(abAry, $"{Application.dataPath}/Resources/");
        //构建AB包放到输出目录下 Application.streamingAssetsPath
        BuildPipeline.BuildAssetBundles(abOutPath, abList.ToArray(), BuildAssetBundleOptions.None, target);
        */

        //加载补充元数据
        CopyAOTAssembliesToDestinationPath();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    /*
    //rootPath dir pack to a bundle
    public static void PackDirToABundle(List<AssetBundleBuild> abList, string rootPath)
    {
        DirectoryInfo directionInfo = new DirectoryInfo(rootPath);
        List<string> otherAssetList = new List<string>();
        List<string> sceneList = new List<string>();
        SLog.Log("打包文件夹下的Asset和Scene：" + directionInfo.Parent.Name);
        FileInfo[] fileinfos = directionInfo.GetFiles("*", SearchOption.AllDirectories);
        SLog.Log("目录下文件个数：" + fileinfos.Length);
        if (fileinfos.Length == 0)
        {
            SLog.Warn("dir has no files not pack to ab：" + rootPath);
            return;
        }

        SLog.Log("begin pack bd " + directionInfo.Parent.Name);
        foreach (var item in fileinfos)
        {
            if (item.Name.EndsWith(".meta") || item.Name.EndsWith(".DS_Store"))
            {
                continue;
            }
            SLog.Log("打包文件：" + item.Name);
            bool isScene = item.Name.EndsWith(".unity");
            List<string> addList = isScene ? sceneList : otherAssetList;
            addList.Add(item.FullName.Replace("\\", "/"));
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        if (sceneList.Count > 0)
        {
            AssetBundleBuild assetSc = new AssetBundleBuild
            {
                assetBundleName = directionInfo.Parent.Name + "Sc",
                assetNames = sceneList.Select(s => ToRelativeAssetPath(s)).ToArray(),
            };

            abList.Add(assetSc);
        }

        if (otherAssetList.Count > 0)
        {
            AssetBundleBuild assetOther = new AssetBundleBuild
            {
                assetBundleName = directionInfo.Parent.Name,//directionInfo.Name,
                assetNames = otherAssetList.Select(s => ToRelativeAssetPath(s)).ToArray(),
            };

            abList.Add(assetOther);
        }
    }


    //every sub dir to a bundle
    public static void PackDirSubDirToABundle(List<AssetBundleBuild> abList, string rootPath)
    {
        DirectoryInfo directionInfo = new DirectoryInfo(rootPath);
        DirectoryInfo[] directions = directionInfo.GetDirectories();
        //每个目录打一个AB包
        for (int i = 0; i < directions.Length; i++)
        {
            List<string> dirAssetsList = new List<string>();
            SLog.Log("打包文件夹：" + directions[i].Name);
            FileInfo[] fileinfos = directions[i].GetFiles("*", SearchOption.AllDirectories);
            SLog.Log("目录下文件个数：" + fileinfos.Length);
            foreach (var item in fileinfos)
            {
                if (item.Name.EndsWith(".meta") || item.Name.EndsWith(".DS_Store"))
                {
                    continue;

                }
                SLog.Log("打包文件：" + item.Name);
                dirAssetsList.Add(item.FullName.Replace("\\", "/"));
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            AssetBundleBuild assetBd = new AssetBundleBuild
            {
                assetBundleName = directions[i].Name,
                assetNames = dirAssetsList.Select(s => ToRelativeAssetPath(s)).ToArray(),
            };
            abList.Add(assetBd);
        }
    }
    */
    //aot 加.bytes并复制到对应目录 目前是output path
    public static void CopyAOTAssembliesToDestinationPath()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotAssembliesDstDir = codeOutPath;//Application.streamingAssetsPath;
        List<string> aotListWithDll = AOTMetaAssemblyNames.Select(fileName => fileName + ".dll").ToList();

        foreach (var dllName in aotListWithDll)
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dllName}";
            if (!File.Exists(srcDllPath))
            {
                SLog.Err($"copy aot dll to hot directory failed <file not exist> :{srcDllPath} ");
                continue;
            }
            string dllBytesPath = $"{aotAssembliesDstDir}/{dllName}.bytes";
            File.Copy(srcDllPath, dllBytesPath, true);
            SLog.Log($"copy aot file to dst dir succeed {srcDllPath} -> {dllBytesPath}");
        }
    }


    // public static string ToRelativeAssetPath(string s)
    // {
    //     string path = s.Substring(s.IndexOf("Assets/"));
    //     SLog.Log(path);
    //     return path;
    // }


    // [MenuItem("ABTool/生成version")]
    // public static void MakeVersion()
    // {
    //     string json = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Config/Config.json").text;
    //     Config config = JsonConvert.DeserializeObject<Config>(json);
    //     List<VersionData> versionDatas = new List<VersionData>();
    //     //第一条是假数据，用来判断是否需要热更新
    //     VersionData version = new VersionData();
    //     //判断使用测试版还是正式版热更地址
    //     config.upid++;
    //     version.abName = config.isBate ? config.hotPath_Bate : config.hotPath;
    //     version.len = config.upid;//更新号
    //     version.md5 = config.version;//版本号
    //     versionDatas.Add(version);
    //     json = JsonConvert.SerializeObject(config);
    //     File.WriteAllText(Application.dataPath + "/Resources/Config/Config.json", json);

    //     //找到S目录下所有文件
    //     string[] files = Directory.GetFiles(Application.streamingAssetsPath, ".", SearchOption.AllDirectories);

    //     foreach (var item in files)
    //     {
    //         //取后缀名
    //         string extensionName = Path.GetExtension(item);
    //         //排除.meta和.manifest文件
    //         if (extensionName.Contains("meta") || extensionName.Contains("manifest")) continue;
    //         SLog.easyLog(item);
    //         string abName = item.Replace(Application.dataPath, "Assets");
    //         abName = abName.Replace("\\", "/");
    //         string assetBundleName = "";
    //         assetBundleName = abName.Replace("Assets/StreamingAssets/", string.Empty);

    //         VersionData versionData = new VersionData();

    //         versionData.abName = assetBundleName;
    //         versionData.len = File.ReadAllBytes(item).Length;
    //         versionData.md5 = FileMD5(item);
    //         versionDatas.Add(versionData);

    //     }
    //     //写入文件
    //     string versionInfo = JsonConvert.SerializeObject(versionDatas);
    //     File.WriteAllText(Application.streamingAssetsPath + "/version.txt", versionInfo, Encoding.UTF8);
    //     AssetDatabase.Refresh();
    //     SLog.easyLog("version.txt创建成功");
    // }

    // static string FileMD5(string filepath)
    // {
    //     FileStream fileStream = new FileStream(filepath, FileMode.Open);
    //     MD5 mD5 = new MD5CryptoServiceProvider();
    //     byte[] bytes = mD5.ComputeHash(fileStream);
    //     StringBuilder sb = new StringBuilder();
    //     for (int i = 0; i < bytes.Length; i++)
    //     {
    //         sb.Append(bytes[i].ToString("x2"));
    //     }

    //     return sb.ToString();
    // }
}