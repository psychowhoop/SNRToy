using System.Collections.Generic;


namespace SNRDicExtend
{
    public static class DicExtend
    {

        public static void UpdateFromDic<TKey, TValue>(this Dictionary<TKey, TValue> selfDic, Dictionary<TKey, TValue> pasDic)
        {
            foreach (var kvp in pasDic)
            {
                selfDic.UpdateFromKV(kvp.Key, kvp.Value);
            }
        }


        public static void UpdateFromKV<TKey, TValue>(this Dictionary<TKey, TValue> selfDic, TKey key, TValue value)
        {
            if (selfDic.ContainsKey(key))
            {
                selfDic[key] = value; // 如果键已存在，则更新值
            }
            else
            {
                selfDic.Add(key, value); // 如果键不存在，则添加键值对
            }
        }


    }

}