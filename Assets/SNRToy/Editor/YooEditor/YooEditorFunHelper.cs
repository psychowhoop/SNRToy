using System.Collections;
using System.Collections.Generic;
using YooAsset.Editor;

public class YooEditorFunHelper
{

    static public List<string> GetBuildPackageNames()
    {
        List<string> result = new List<string>();
        foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
        {
            result.Add(package.PackageName);
        }

        return result;
    }

    //默认是yoo
    static public string GetYooFolderName()
    {
        string path = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
        string[] folders = path.TrimEnd('/').Split('/'); // 删除结尾的斜杠，并根据斜杠分割路径
        string lastFolderName = folders[folders.Length - 1]; // 获取最后一个部分作为文件夹名称

        return lastFolderName;
    }

}
