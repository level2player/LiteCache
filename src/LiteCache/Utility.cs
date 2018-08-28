using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lsy.core.LiteCache {
    public static partial class Utility {
        /// <summary>
        /// 比较时间差,返回时间差单位毫秒
        /// </summary>
        /// <param name="DateTime1">时间1</param>
        /// <param name="DateTime2">时间2</param>
        /// <returns></returns>
        public static int TimeSecondsDiff (DateTime DateTime1, DateTime DateTime2) {
            int secondsDiff = 0;

            TimeSpan ts1 = new TimeSpan (DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan (DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract (ts2).Duration ();
            for (int i = 0; i < ts.Days; i++) {
                //考虑跨日的情况,每天需要加上86400
                secondsDiff += 86400;
            }
            for (int i = 0; i < ts.Hours; i++) {
                //考虑跨小时的情况,每小时需要加上3600
                secondsDiff += 3600;
            }
            for (int i = 0; i < ts.Minutes; i++) {
                //考虑跨分钟的情况,每分钟需要加上60
                secondsDiff += 60;
            }

            return secondsDiff;
        }

        /// <summary>
        /// 取分割字符后面的字符串
        /// </summary>
        /// <param name="text">原来的文本</param>
        /// <param name="flag">分割字符</param>
        /// <returns></returns>
        public static string GetEndSubString (string text, char flag) {
            var idx = text.LastIndexOf (flag);
            if (idx == -1)
                return string.Empty;
            return text.Substring (idx + 1, text.Length - (idx + 1));
        }
        /// <summary>
        /// 保存缓存到本地dat文件,实际就是一个序列化的过程
        /// </summary>
        /// <param name="dic">数据集合</param>
        /// <param name="dtName">dat文件名称,不需要输入后缀,注意此名称不能重复,不然存在被覆盖的风险</param>
        /// <typeparam name="T">数据类型,如果是对象需要实现序列化接口</typeparam>
        /// <returns>保存结果</returns>
        public static bool SaveCache<T> (Dictionary<string, T> dic, string dtName) {
            if (!Directory.Exists ("cache\\")) {
                Directory.CreateDirectory ("cache\\");
            }
            FileStream fs = new FileStream ("cache\\" + dtName + ".dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter ();
            try {
                formatter.Serialize (fs, dic);
            } catch (SerializationException e) {
                //Console.WriteLine ("Failed to serialize. Reason: " + e.Message);
                throw e;
            } finally {
                fs.Close ();
            }
            return true;
        }
        /// <summary>
        /// 从本地缓存文件取数据,实际就是一个反序列化的过程
        /// </summary>
        /// <param name="dtName">文件名称,不需要输入后缀</param>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>数据集合</returns>
        public static Dictionary<string, T> DeserializeCache<T> (string dtName) {
            FileStream fs = new FileStream ("cache\\" + dtName + ".dat", FileMode.Open);
            try {
                BinaryFormatter formatter = new BinaryFormatter ();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                return (Dictionary<string, T>) formatter.Deserialize (fs);
            } catch (SerializationException e) {
                //Console.WriteLine ("Failed to deserialize. Reason: " + e.Message);
                throw e;
            } finally {
                fs.Close ();
            }
        }
    }
}