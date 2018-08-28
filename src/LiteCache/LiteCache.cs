using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lsy.core.LiteCache {
    public class LiteCache<T> {

        Dictionary<string, T> globalDictionary =
        new Dictionary<string, T> ();

        /// <summary>
        /// 缓存数据集合
        /// </summary>
        /// <typeparam name="string">缓存Key</typeparam>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <returns></returns>
        public Dictionary<string, T> GlobalDictionary {
            get { return globalDictionary; }
        }
        private System.Timers.Timer expiredTimer;

        /// <summary>
        /// 缓存保存时间
        /// </summary>
        /// <value></value>
        public int LiveTime { get; private set; }

        /// <summary>
        /// 检查间隔时间
        /// </summary>
        /// <value></value>
        public int CheckTime { get; private set; }

        /// <summary>
        /// 缓存名称
        /// </summary>
        /// <value></value>
        public string CacheName { get; private set; }

        /// <summary>
        /// 集合行数量
        /// </summary>
        /// <value></value>
        public int Count {
            get {
                return globalDictionary.Count ();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="liveTime">数据生存周期,单位秒</param>
        /// <param name="checkTime">检查数据周期,单位秒</param>
        /// <param name="cacheName">缓存文件名称,不需要输入后缀名</param>
        public LiteCache (int liveTime, int checkTime, string cacheName) {
            LiveTime = liveTime;
            CheckTime = checkTime;
            CacheName = cacheName;
            RegisterTimeCheck ();
        }
        /// <summary>
        /// 从本地dat文件拉数据刷新到缓存中
        /// </summary>
        public void InitLoadLocData () {
            try {
                var dic = Utility.DeserializeCache<T> (CacheName);
                if (dic?.Count > 0) {
                    globalDictionary = dic;
                }
            } catch (System.Exception e) {
                throw e;
            }
        }
        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>guid(唯一标识);是否成功</returns>
        public (string, bool) SetValue (T value) {
            var guid = $"{System.Guid.NewGuid().ToString()}|{System.DateTime.Now.AddSeconds(LiveTime)}";

            globalDictionary.Add (guid, value);
            try {
                Utility.SaveCache (globalDictionary, CacheName);
            } catch (System.Exception e) {
                throw e;
            }

            return (guid, true);
        }

        /// <summary>
        /// 异步设置缓存值
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>guid(唯一标识);是否成功</returns>
        public async Task<(string, bool)> AsynSetValue (T value) {
            return await Task.Run (() => {
                return SetValue (value);
            });
        }

        /// <summary>
        /// 取缓存值
        /// </summary>
        /// <param name="guid">guid(唯一标识)</param>
        /// <returns>数据值;是否成功</returns>
        public (T, bool) GetValue (string guid) {
            if (string.IsNullOrEmpty (guid))
                return (default (T), false);

            T result = default (T);

            if (globalDictionary.TryGetValue (guid, out result))
                return (result, true);
            else {
                try {
                    var dic = Utility.DeserializeCache<T> (CacheName);
                    if (dic?.Count > 0) {
                        if (dic.TryGetValue (guid, out result))
                            return (result, true);
                    }
                    return (default (T), false);
                } catch (System.Exception) {
                    return (result, false);
                }
            }
        }
        /// <summary>
        /// 异步取缓存值
        /// </summary>
        /// <param name="guid">guid(唯一标识)</param>
        /// <returns>数据值;是否成功</returns>
        public async Task<(T, bool)> AsynGetValue (string guid) {
            return await Task.Run (() => {
                return GetValue (guid);
            });
        }

        /// <summary>
        /// 更新缓存值
        /// </summary>
        /// <param name="guid">guid(委托编号)</param>
        /// <param name="value">需要更新的值</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateValue (string guid, T value) {
            if (string.IsNullOrEmpty (guid))
                return false;
            if (globalDictionary.TryGetValue (guid, out var result)) {
                result = value;
                try {
                    return Utility.SaveCache (globalDictionary, CacheName);
                } catch (System.Exception e) {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// 异步更新缓存值
        /// </summary>
        /// <param name="guid">guid(委托编号)</param>
        /// <param name="value">需要更新的值</param>
        /// <returns>是否更新成功</returns>
        public async Task<bool> AsynUpdateValue (string guid, T value) {
            return await Task.Run (() => {
                return UpdateValue (guid, value);
            });
        }

        /// <summary>
        /// 删除缓存值
        /// </summary>
        /// <param name="guid">guid(唯一标识)</param>
        /// <returns>是否成功</returns>
        public bool DeleteValue (string guid) {
            if (string.IsNullOrEmpty (guid))
                return false;
            if (globalDictionary.Remove (guid)) {
                try {
                    return Utility.SaveCache (globalDictionary, CacheName);
                } catch (System.Exception ex) {
                    throw ex;
                }
            }
            return false;
        }
        /// <summary>
        /// 异步删除缓存值
        /// </summary>
        /// <param name="guid">guid(唯一标识)</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AsynDeleteValue (string guid) {
            return await Task.Run (() => {
                return DeleteValue (guid);
            });
        }

        /// <summary>
        /// 删除集合数据,并且删除本地缓存
        /// </summary>
        /// <returns>删除结果</returns>
        public bool Flush () {
            if (globalDictionary?.Count () > 0) {
                globalDictionary.Clear ();
                try {
                    return Utility.SaveCache (globalDictionary, CacheName);
                } catch (System.Exception ex) {

                    throw ex;
                }
            }
            return false;

        }

        #region 私有方法

        private void RegisterTimeCheck () {
            expiredTimer = new Timer (CheckTime * 1000);
            expiredTimer.Elapsed += OnTimedEvent;
            expiredTimer.AutoReset = true;
            expiredTimer.Enabled = true;
        }

        private void OnTimedEvent (Object source, ElapsedEventArgs e) {

            List<string> clear_list = new List<string> ();

            foreach (string keyColl in globalDictionary.Keys) {
                var key_dt = Utility.GetEndSubString (keyColl, '|');

                if (string.IsNullOrEmpty (key_dt))
                    continue;

                var data_dt = Convert.ToDateTime (key_dt);

                if (data_dt >= DateTime.Now)
                    continue;

                if (Utility.TimeSecondsDiff (data_dt, DateTime.Now) >= 0)
                    clear_list.Add (keyColl);
            }
            bool isClear = false;
            foreach (string keyColl in clear_list) {
                isClear = globalDictionary.Remove (keyColl);
                //Console.WriteLine ($"Key:{keyColl}  is expired");
            }
            if (isClear) {
                try {
                    Utility.SaveCache (globalDictionary, CacheName);
                } catch (System.Exception ex) {
                    Console.WriteLine ("save cache error,info=", ex.Message);
                    //throw ex;
                }
            }
        }
        #endregion
    }
}