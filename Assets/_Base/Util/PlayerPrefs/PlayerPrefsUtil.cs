using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public class PlayerPrefsUtil
    {
        public const char split = ',';

        public static void SetStrings<T>(string key, IEnumerable<T> datas, char split = PlayerPrefsUtil.split)
        {
            if (datas == null)
            {
                PlayerPrefs.SetString(key, "");
            }
            else
            {
                string s = string.Join(split.ToString(), datas);
                PlayerPrefs.SetString(key, s);
            }
        }

        public static List<string> GetStrings(string key)
        {
            List<string> result = new List<string>();

            string data = PlayerPrefs.GetString(key);

            if (string.IsNullOrEmpty(data))
            {
                return result;
            }

            string[] datas = data.Split(split);
            result = new List<string>(datas);

            return result;
        }

        public static List<int> GetInts(string key , char split = PlayerPrefsUtil.split)
        {
            List<int> result = new List<int>();

            string data = PlayerPrefs.GetString(key);

            if (string.IsNullOrEmpty(data))
            {
                return result;
            }

            string[] datas = data.Split(split);
            foreach (string item in datas)
            {
                try
                {
                    int v = int.Parse(item.Trim());
                    result.Add(v);
                }
                catch (System.Exception e)
                {
                    Log.E("PlayerPrefsUtil", item + " can not parse to int, error:" + e.Message);
                }
            }

            return result;
        }

        public static void SetLong(string key, long value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static long GetLong(string key)
        {
            return System.Convert.ToInt64(PlayerPrefs.GetString(key));
        }
    }
}
