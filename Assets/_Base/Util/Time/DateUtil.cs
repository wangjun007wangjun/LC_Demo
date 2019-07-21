using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace BaseFramework
{
    public class DateUtil
    {
        public static string defaultDateFormat = "yyyy-MM-dd HH:mm:ss";
        public static readonly DateTime defaultDateTime = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis(DateTime? dateTime, bool useUtc = false)
        {
            if (dateTime == null)
                return (long) (DateTime.UtcNow - defaultDateTime).TotalMilliseconds;
            return (long) ((useUtc ? dateTime.Value.ToUniversalTime() : dateTime.Value) - defaultDateTime).TotalMilliseconds;
        }

        public static DateTime GetDateTime(string dateString, string format = null)
        {
            return GetDateTime(dateString, format, CultureInfo.InvariantCulture);
        }

        public static DateTime GetDateTime(string dateString, string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return defaultDateTime;
            }

            //防止错误的格式直接调用DateTime.ParseExact崩溃. eg. dateString = "20180719"
            dateString = FixDateString(dateString);
            
            if (string.IsNullOrEmpty(dateString))
            {
                return defaultDateTime;
            }

            if (string.IsNullOrEmpty(format))
            {
                format = defaultDateFormat;
            }

            try
            {
                return DateTime.ParseExact(dateString, format, provider, DateTimeStyles.None);
            }
            catch 
            {

                return defaultDateTime;
            }
        }


        public static string GetDateString(DateTime dateTime, string format = null)
        {
            return GetDateString(dateTime, format, CultureInfo.InvariantCulture);
        }

        public static string GetDateString(DateTime dateTime, string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = defaultDateFormat;
            }
            return dateTime.ToString(defaultDateFormat, provider);
        }

        public static TimeSpan GetTimeSpan(string timeSpanString)
        {
            if (string.IsNullOrEmpty(timeSpanString))
            {
                return TimeSpan.Zero;
            }
            try
            {
                string[] strs = timeSpanString.Split(':');
                int h = int.Parse(strs[0]);
                int m = int.Parse(strs[1]);
                int s = int.Parse(strs[2]);
                return new TimeSpan(h, m, s);
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }


        public static string GetTimeSpanString(TimeSpan timeSpan)
        {
            return
                $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        //支持解析以下格式
        // "2016-07-31"
        //"20160106121310"
        //"20160106"
        //"2016-01-06"
        //"2016-01-06 12:13:10"
        //"2016年01月06日12点13分10秒"
        //"2016年01月06日 12:13:10"
        //"2016/01/06 12:13:10"
        private static int[] getTimeInfoFromString(string strTime, ref int timeInfoIndex)
        {
            if (string.IsNullOrEmpty(strTime))
                return null;

            int[] timeInfo = new int[7];//年 月 日 时 分 秒 毫秒

            timeInfoIndex = 0;

            int nLen = strTime.Length;
            int iBegin = -1;
            int iEnd = -1;

            for (int i = 0; i < nLen; ++i)
            {
                if (timeInfoIndex >= timeInfo.Length)
                    break;

                bool bGetNumber = false;
                char c = strTime[i];
                if (c < '0' || c > '9')
                {
                    if (iBegin >= 0 && i > iBegin)
                    {
                        bGetNumber = true;
                        iEnd = i;
                    }
                }
                else
                {
                    if (iBegin == -1)
                    {
                        iBegin = i;
                    }
                    else if (i == nLen - 1)
                    {
                        bGetNumber = true;
                        iEnd = i + 1;
                    }
                }

                if (!bGetNumber) continue;
                
                int iFrom = iBegin;
                iBegin = -1;

                int nValueLen = iEnd - iFrom;

                if (timeInfoIndex == 0)
                {
                    if (nValueLen >= 8 && nValueLen <= 17)
                    { //e.g 20160203161059
                        timeInfo[timeInfoIndex++] = Convert.ToInt32(strTime.Substring(iFrom, 4));
                        timeInfo[timeInfoIndex++] = Convert.ToInt32(strTime.Substring(iFrom + 4, 2));
                        timeInfo[timeInfoIndex++] = Convert.ToInt32(strTime.Substring(iFrom + 6, 2));

                        iFrom = iFrom + 8;
                        nValueLen = nValueLen - 8;
                        int j = 0;
                        for (; j < 3; ++j)
                        {
                            if (nValueLen >= 2)
                            {
                                timeInfo[timeInfoIndex++] = Convert.ToInt32(strTime.Substring(iFrom, 2));
                                iFrom = iFrom + 2;
                                nValueLen = nValueLen - 2;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (j == 3 && nValueLen >= 3)
                        {
                            timeInfo[timeInfoIndex++] = Convert.ToInt32(strTime.Substring(iFrom, iEnd - iFrom));
                        }
                        continue;
                    }
                }
                else
                {
                    if (nValueLen > 4)
                        return null;
                }

                int value = Convert.ToInt32(strTime.Substring(iFrom, iEnd - iFrom));
                timeInfo[timeInfoIndex++] = value;
            }
            //check valid
            if (timeInfoIndex < 3)
                return null;
            if (timeInfo[3] > 24)
                return null;
            if (timeInfo[4] > 60)
                return null;
            return timeInfo[5] > 60 ? null : timeInfo;
        }

        private static string FixDateString(string dateString)
        {
            int timeInfoIndex = 0;
            int[] timeInfo = getTimeInfoFromString(dateString, ref timeInfoIndex);
            if (timeInfo == null)
                return "";
            return timeInfoIndex < 6
                       ? $"{timeInfo[0]:D4}-{timeInfo[1]:D2}-{timeInfo[2]:D2} {timeInfo[3]:D2}:{timeInfo[4]:D2}:{timeInfo[5]:D2}"
                       : dateString;
        }
    }
}
