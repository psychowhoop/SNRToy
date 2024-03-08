using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SNRDicExtend;
using SNRLogHelper;
using SNRStringExtend;



namespace SNRFileHelper
{

    public static class FileHelper
    {
        //if true not delete the root folder
        public static void DeleteFolder(string rootPath, bool keepRootFolder = false)
        {
            if (!Directory.Exists(rootPath))
            {
                SLog.Log("dir not exist not need delete == " + rootPath);
                return;
            }


            if (keepRootFolder)
            {
                // 删除文件夹下的所有文件
                foreach (string filePath in Directory.GetFiles(rootPath))
                {
                    File.Delete(filePath);
                }

                // 删除文件夹下的所有子文件夹
                foreach (string subFolderPath in Directory.GetDirectories(rootPath))
                {
                    Directory.Delete(subFolderPath, true);
                }

            }
            else
            {
                Directory.Delete(rootPath, true);
            }
        }


        //if file not exist ,create it. true create new file, false file already exist
        public static bool CreateFile(string fullPath, string content = "")
        {
            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, content);
                return true;
            }

            return false;
        }

        //if file not exist will create file
        public static void WriteJsonFile<TKey, TValue>(string fullPath, Dictionary<TKey, TValue> pasDicData, bool overWrite = true, bool format = true)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = format ? Formatting.Indented : Formatting.None // 添加缩进，提高可读性
            };

            string writeStrData = JsonConvert.SerializeObject(pasDicData, settings);
            if (CreateFile(fullPath, writeStrData))
            {
                return;
            }

            if (!overWrite)
            {
                Dictionary<TKey, TValue> cacheDic = ReadJsonFile<TKey, TValue>(fullPath);
                cacheDic.UpdateFromDic(pasDicData);
                writeStrData = JsonConvert.SerializeObject(cacheDic, settings);
            }

            File.WriteAllText(fullPath, writeStrData);
        }


        //fullPath with file name extension
        public static Dictionary<TKey, TValue> ReadJsonFile<TKey, TValue>(string fullPath)
        {
            // 读取文件Txt内容
            string jsonStr = File.ReadAllText(fullPath);

            // 将JSON字符串解析为Dictionary
            Dictionary<TKey, TValue> dic = jsonStr.ToDic<TKey, TValue>();

            return dic;
        }




    }

}



