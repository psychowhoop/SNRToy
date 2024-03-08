using System;
using System.IO;
using UnityEngine;
using YooAsset.Editor;
using SNRAryExtend;
using UnityEditor;
using SNRLogHelper;
using SNRKWordDefine;
using System.Collections.Generic;
using SNRStringExtend;
using SNRDicExtend;
using Newtonsoft.Json;
using SNRFileHelper;
using YooAssetHelper;




//only the ios platform tested

namespace OneStopBuild
{
    [System.Serializable]
    public class PkgModel
    {
        public string name;
        public string version;
        public EBuildPipeline pipeline;
        public bool build;

        public PkgModel(string name)
        {
            this.name = name;
            this.version = "0.0.0";
            this.build = true;
            this.pipeline = EBuildPipeline.BuiltinBuildPipeline;

        }

    }


    public class PkgListWindow : EditorWindow
    {
        private Vector2 mScrollPosition = Vector2.zero;
        private List<PkgModel> mPkgList = new List<PkgModel>();
        public static bool sCanBuild = true;
        public static string sHotCopyDir = Path.Combine(EditorTools.GetProjectPath(), "YooHotFile");


        private void OnEnable()
        {
            UpdatePkgListFromCache();
        }

        void UpdatePkgListFromCache()
        {
            mPkgList.Clear();
            Dictionary<string, PkgModel> cachePkgInfoDic = PkgListWindow.GetCachePkgDic();
            //当前yooasset中设置的pkg
            var curPkgNameList = YooEditorFunHelper.GetBuildPackageNames();
            foreach (string pkgName in curPkgNameList)
            {
                PkgModel model = cachePkgInfoDic.GetValueOrDefault(pkgName, null);
                model = model != null ? model : new PkgModel(pkgName);
                mPkgList.Add(model);
            }
        }




        public static Dictionary<string, PkgModel> GetCachePkgDic()
        {
            string cacheInfoStr = EditorPrefs.GetString(KWord.PkgBuildInfo, null);
            Dictionary<string, PkgModel> cachePkgInfoDic = string.IsNullOrEmpty(cacheInfoStr) ? new Dictionary<string, PkgModel>() : cacheInfoStr.ToDic<string, PkgModel>();

            return cachePkgInfoDic;
        }

        public static void UpdatePkgDic(List<PkgModel> modelList)
        {
            Dictionary<string, PkgModel> cachePkgInfoDic = PkgListWindow.GetCachePkgDic();
            foreach (PkgModel model in modelList)
            {
                cachePkgInfoDic.UpdateFromKV(model.name, model);
            }
            string cacheStr = JsonConvert.SerializeObject(cachePkgInfoDic);
            EditorPrefs.SetString(KWord.PkgBuildInfo, cacheStr);
        }


        public static void UpdateJsonVersionFile(List<PkgModel> modelList)
        {
            string versionFilePath = Path.Combine(PkgListWindow.sHotCopyDir, "version.json");

            Dictionary<string, string> verDic = new Dictionary<string, string>();
            foreach (PkgModel model in modelList)
            {
                if (model.build)
                {
                    verDic.Add(model.name, model.version);
                }
            }

            FileHelper.WriteJsonFile(versionFilePath, verDic, false);
            SLog.Log("update version file complete~~~~~~~");
        }


        [MenuItem("YooAsset/OneStopBuild")]
        public static void ShowWindow()
        {
            sCanBuild = true;
            EditorWindow.GetWindow(typeof(PkgListWindow), false, "OneDragonService", true);
        }



        private void OnGUI()
        {
            GUILayout.Label("PipeType 0-Builtin 1-Scriptable 2-RawFile", EditorStyles.boldLabel);

            // 开始可滚动区域
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, GUILayout.Width(500), GUILayout.Height(500));

            // 遍历数组中的每个元素
            for (int i = 0; i < mPkgList.Count; i++)
            {
                PkgModel model = mPkgList[i];

                // 显示单选框和名称编辑框
                GUILayout.BeginHorizontal();
                model.build = GUILayout.Toggle(model.build, "Build:") ? true : false;
                GUILayout.Label("Name:");
                model.name = EditorGUILayout.TextField(model.name);
                GUILayout.Label("Version:");
                model.version = EditorGUILayout.TextField(model.version);
                GUILayout.Label("PipeLine:");
                model.pipeline = EditorGUILayout.TextField((int)model.pipeline + "").ToEnum<EBuildPipeline>();

                GUILayout.EndHorizontal();

                // 更新数组中的数据
                mPkgList[i] = model;
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10); // 空白间距

            // 创建按钮

            if (GUILayout.Button("Build", GUILayout.Width(300), GUILayout.Height(100)))
            {
                if (sCanBuild)
                {
                    sCanBuild = false;
                    // 更新数据
                    StartBuild();
                }
            }

            if (GUILayout.Button("ClearVerCache", GUILayout.Width(100), GUILayout.Height(50)))
            {
                ClearVersionCacheInfo();
            }

            if (GUILayout.Button("ClearAllVerFile", GUILayout.Width(100), GUILayout.Height(50)))
            {
                // 显示确认对话框
                bool result = EditorUtility.DisplayDialog("FBI Warning", "It will clear all Cached Version File(only current platform)", "就是要删", "怂了");

                if (result)
                {
                    SLog.Log("begin clear all cached version file");
                    string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
                    string projPath = EditorTools.GetProjectPath();
                    string assetPath = Application.dataPath;
                    string streamAssetYooPath = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
                    string hotCode = Path.Combine(assetPath, "AllToHot/HotCode");
                    string pkgBundlePath = Path.Combine(AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(), platform);
                    string hyHotDLL = Path.Combine(projPath, $"HybridCLRData/HotUpdateDlls/{platform}");
                    string hy2Cpp = Path.Combine(projPath, $"HybridCLRData/AssembliesPostIl2CppStrip/{platform}");
                    string hyTmpProj = Path.Combine(projPath, $"HybridCLRData/StrippedAOTDllsTempProj/{platform}");
                    string hyTmpProj2 = Path.Combine(projPath, $"HybridCLRData/StrippedAOTDllsTempProj/{platform}_BurstDebugInformation_DoNotShip");
                    string rootYoo = Path.Combine(projPath, YooEditorFunHelper.GetYooFolderName());

                    List<string> delFolderPath = new List<string>
                    {
                        hotCode,
                        streamAssetYooPath,
                        pkgBundlePath,
                        hyHotDLL,
                        hy2Cpp,
                        hyTmpProj,
                        hyTmpProj2,
                        rootYoo,
                    };

                    foreach (string delPath in delFolderPath)
                    {
                        FileHelper.DeleteFolder(delPath, true);
                    }

                    SLog.Log("delete all hot version file succeed");

                }
                else
                {
                    // 用户选择 NO，取消操作
                    SLog.Log("not clear now");
                }

            }


        }

        public void ClearVersionCacheInfo()
        {
            Dictionary<string, PkgModel> dic = PkgListWindow.GetCachePkgDic();
            string str = JsonConvert.SerializeObject(dic);
            SLog.Log("cur cache data is " + str);
            EditorPrefs.SetString(KWord.PkgBuildInfo, "");
            UpdatePkgListFromCache();
            SLog.Log("clear complete");

        }


        private void StartBuild()
        {
            EditorTools.ClearUnityConsole();
            SLog.Log("begin deal hybrid");
            HybridDealDll.SNRHybridOneClick();
            SLog.Log("end deal hybrid");

            SLog.Log("begin deal yoo");
            DealYoo();
            SLog.Log("end deal yoo");

            SLog.Log("everything is complete~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        private void DealYoo()
        {
            string pkgInfoStr = JsonConvert.SerializeObject(mPkgList);
            SLog.Log(pkgInfoStr);

            EditorApplication.delayCall += () => PkgBuildAction.JustDoIt(mPkgList);
        }





    }


    public class PkgBuildAction
    {

        public static void CopyHotFileToDir()
        {
            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            SLog.Log("Active Build Target: " + activeBuildTarget);
            SLog.Log("begin copy hot file");
            string projectPath = EditorTools.GetProjectPath();
            string rootPath = PkgListWindow.sHotCopyDir;

            // 创建根目录文件夹
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            string srcPath = Path.Combine(projectPath, "Bundles", activeBuildTarget.ToString());
            if (!Directory.Exists(srcPath))
            {
                Debug.LogError("src Path does not exist: " + srcPath);
                return;
            }

            string[] srcFolders = Directory.GetDirectories(srcPath);
            foreach (string folderPath in srcFolders)
            {
                string folderName = Path.GetFileName(folderPath);
                string targetFolderPath = Path.Combine(rootPath, activeBuildTarget.ToString() + "/" + folderName);

                // 如果目标文件夹不存在，则创建
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                // 检查除了OutputCache以外的子文件夹
                string[] subFolders = Directory.GetDirectories(folderPath);
                foreach (string subFolder in subFolders)
                {
                    string subFolderName = Path.GetFileName(subFolder);
                    if (subFolderName != "OutputCache")
                    {
                        string destinationFolder = Path.Combine(targetFolderPath, subFolderName);

                        // 如果目标文件夹存在，则删除
                        if (Directory.Exists(destinationFolder))
                        {
                            Directory.Delete(destinationFolder, true);
                        }

                        // 复制文件夹
                        CopyFolder(subFolder, destinationFolder);
                    }
                }
            }
            EditorUtility.RevealInFinder(rootPath);
            SLog.Log("end copy file");
        }



        // 复制文件夹的函数
        private static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceFolder);
            if (!dir.Exists)
            {
                Debug.LogError("Source folder does not exist: " + sourceFolder);
                return;
            }

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            // 复制文件
            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destinationFolder, fileName);
                File.Copy(filePath, destFilePath, true);
            }

            // 递归复制子文件夹
            foreach (string subFolder in Directory.GetDirectories(sourceFolder))
            {
                string folderName = Path.GetFileName(subFolder);
                string destSubFolder = Path.Combine(destinationFolder, folderName);
                CopyFolder(subFolder, destSubFolder);
            }
        }



        public static void JustDoIt(List<PkgModel> pkgList)
        {
            SLog.Log("begin list build +++++++++++++++");
            for (int i = 0; i < pkgList.Count; ++i)
            {
                PkgModel model = pkgList[i];
                if (model.build)
                {
                    BuildPackage(model.name, model.version, model.pipeline);
                }

            }

            //write to cache info
            PkgListWindow.UpdatePkgDic(pkgList);
            PkgListWindow.UpdateJsonVersionFile(pkgList);

            SLog.Log("end list build +++++++++++++++++");
            CopyHotFileToDir();
            SLog.Log("copy file to hot dir end");
            PkgListWindow.sCanBuild = true;
            SLog.Log("all hot operation complete~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }




        public static void BuildPackage(string pkgName, string pkgVer, EBuildPipeline pipelineType = EBuildPipeline.BuiltinBuildPipeline)
        {
            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            SLog.Log("Active Build Target: " + activeBuildTarget);
            SLog.Log("begin build pkg " + pkgName + " ver " + pkgVer + " ppType " + pipelineType.ToString() + " target " + activeBuildTarget);
            var buildMode = EBuildMode.ForceRebuild;
            var fileNameStyle = EFileNameStyle.BundleName;
            var buildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;
            var buildinFileCopyParams = "";

            BuildParameters buildParameters = null;
            IBuildPipeline pipeline = null;

            switch (pipelineType)
            {
                case EBuildPipeline.BuiltinBuildPipeline:
                    {
                        BuiltinBuildParameters usePara = new BuiltinBuildParameters();
                        buildParameters = usePara;

                        buildParameters.EnableSharePackRule = true;
                        usePara.CompressOption = ECompressOption.LZ4;

                        pipeline = new BuiltinBuildPipeline();

                    }
                    break;

                case EBuildPipeline.RawFileBuildPipeline:
                    {

                        buildParameters = new RawFileBuildParameters();

                        pipeline = new RawFileBuildPipeline();

                    }
                    break;

                case EBuildPipeline.ScriptableBuildPipeline:
                    {
                        //heywait

                    }
                    break;


            }

            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = pipelineType.ToString();
            buildParameters.BuildTarget = activeBuildTarget;
            buildParameters.BuildMode = buildMode;
            buildParameters.PackageName = pkgName;
            buildParameters.PackageVersion = pkgVer;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = fileNameStyle;
            buildParameters.BuildinFileCopyOption = buildinFileCopyOption;
            buildParameters.BuildinFileCopyParams = buildinFileCopyParams;
            //启用会报类型找不到错误 目前没有加密服务暂时为null
            //buildParameters.EncryptionServices = CreateEncryptionInstance(pkgName);

            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                SLog.Log("build " + buildResult.OutputPackageDirectory + " succeed");
                //EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            }

            SLog.Log("end build pkg " + pkgName);
        }



        /// <summary>
        /// 创建加密类实例
        /// </summary>
        // protected static IEncryptionServices CreateEncryptionInstance(string pkgName)
        // {
        //     var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(pkgName, BuildPipeline);
        //     var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
        //     var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
        //     if (classType != null)
        //         return (IEncryptionServices)Activator.CreateInstance(classType);
        //     else
        //         return null;
        // }

    }

}

