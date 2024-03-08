using System;
using System.IO;
using SNRAryExtend;
using UnityEngine;
using YooAsset.Editor;



public class SNRFilterRuleData
{
    public string[] filterFolderAry;//if directory in path contains the name in ary,will not added 
}


//"Assets/AllToHot/Base/Asset/Tex/Load/bgLoadStarAni.png" data.assetpath
//eg:{"filterFolderAry":["Spine"]}  //在楼上的路径格式中,如果有Spine名字的文件夹存在,则该路径的资源过滤掉不加入收集资源列表
[DisplayName("根据userData-json自定义收集 没设置userData则全收集")]
public class CollectSNR : IFilterRule
{
    public bool IsCollectAsset(FilterRuleData data)
    {
        if (data.UserData != null && !data.UserData.Equals(string.Empty))
        {
            SNRFilterRuleData filterData = JsonUtility.FromJson<SNRFilterRuleData>(data.UserData);
            string[] filterFolderAry = filterData.filterFolderAry;
            string[] dirAry = data.AssetPath.Split('/');
            int segLen = dirAry.Length;
            if (segLen > 0)
            {//only check directory
                string lastSeg = dirAry[segLen - 1];
                if (!string.IsNullOrEmpty(Path.GetExtension(lastSeg)))
                {
                    Array.Resize(ref dirAry, segLen - 1);
                }
            }

            bool hasSameEle = filterFolderAry.ContainSameEleInAry(dirAry);

            return !hasSameEle;
        }

        //全收集
        return true;

    }
}