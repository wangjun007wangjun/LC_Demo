using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace BaseFramework
{

    public class TimeUtil
    {

        private const string WEB_TIME_URL = "http://www.google.com";

        private static bool _alreadyLoadNetTime;
        private static DateTime _netTime;
        private static float _realtimeSinceGetNetTime;

        private static bool _isFectching = false;

        public static void FetchNetTime(Action<DateTime> onFetched,
                                               Action<string> onFailed = null,
                                               bool force = false)
        {
            TaskHelper.Create<CoroutineTask>()
                .Delay(Fetch(onFetched, onFailed, force))
                .Execute();
        }

        /// <summary>  
        /// 获取网页提供的时间
        /// 通过分析网页报头，查找Date对应的值，获得GMT格式的时间，并转化为本地时间传递给onFetched回调
        /// 该接口在协程中异步执行，不会阻塞UI
        /// </summary>
        private static IEnumerator Fetch(Action<DateTime> onFetched,
                                               Action<string> onFailed = null,
                                               bool force = false)
        {

            if (!force && _alreadyLoadNetTime)
            {
                yield return null;
                
                onFetched?.Invoke(_netTime.AddSeconds(Time.realtimeSinceStartup - _realtimeSinceGetNetTime));
            }
            else if (Debug.unityLogger.logEnabled)
            {
                yield return null;

                _netTime = DateTime.Now;
                _realtimeSinceGetNetTime = Time.realtimeSinceStartup;
                _alreadyLoadNetTime = true;
                onFetched?.Invoke(_netTime);
            }
            else
            {
                if (_isFectching)
                {
                    onFailed?.Invoke("isFectching");
                    yield break;
                }
                _isFectching = true;

                UnityWebRequest request = new UnityWebRequest(WEB_TIME_URL);
                yield return request.SendWebRequest();
                if (!string.IsNullOrEmpty(request.error))
                {
                    onFailed?.Invoke(request.error);
                }
                else if (request.GetResponseHeaders() == null)
                {
                    onFailed?.Invoke("Response headers is empty!");
                }
                else
                {
                    string gmt = request.GetResponseHeader("Date");

                    if (string.IsNullOrEmpty(gmt))
                    {
                        onFailed?.Invoke("Invalid response headers of url: " + request.url);
                    }
                    else
                    {
                        _netTime = GMT2Local(gmt);
                        _realtimeSinceGetNetTime = Time.realtimeSinceStartup;
                        _alreadyLoadNetTime = true;
                        onFetched?.Invoke(_netTime);
                    }
                }
            }
        }

        /// <summary>    
        /// GMT时间转成本地时间   
        /// DateTime dt1 = GMT2Local("Thu, 29 Sep 2014 07:04:39 GMT");
        /// 转换后的dt1为：2014-9-29 15:04:39  
        /// DateTime dt2 = GMT2Local("Thu, 29 Sep 2014 15:04:39 GMT+0800");  
        /// 转换后的dt2为：2014-9-29 15:04:39
        /// </summary>    
        private static DateTime GMT2Local(string gmt)
        {
            Debug.Log("[GMT2Local] GMT Time : " + gmt);

            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.Contains("+0"))
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().Contains("GMT"))
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, 
                                             pattern, 
                                             CultureInfo.InvariantCulture,
                                             DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to convert GMT to local date time: " + e.Message);
            }

            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

            return dt;
        }

        /// <summary>    
        /// 本地时间转成GMT时间    
        /// string s = ToGMTString(DateTime.Now);  
        /// 本地时间为：2014-9-29 15:04:39  
        /// 转换后的时间为：Thu, 29 Sep 2014 07:04:39 GMT  
        /// </summary>
        public static string LocalToGMTString(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("r");
        }

        /// <summary>
        /// 本地时间转成GMT格式的时间  
        /// string s = ToGMTFormat(DateTime.Now);  
        /// 本地时间为：2014-9-29 15:04:39  
        /// 转换后的时间为：Thu, 29 Sep 2014 15:04:39 GMT+0800  
        /// </summary>
        public static string LocalToGMTFormat(DateTime dt)
        {
            return dt.ToString("r") + dt.ToString("zzz").Replace(":", "");
        }

        private static readonly DateTime JAN1_ST1970 = new DateTime(
                                                                    1970, 
                                                                    1, 
                                                                    1, 
                                                                    0, 
                                                                    0, 
                                                                    0, 
                                                                    DateTimeKind.Utc);

        public static long GetTimeMillis(DateTime time)
        {
            return (time.ToUniversalTime().Ticks - JAN1_ST1970.Ticks) / 10000;
        }
    }
}
