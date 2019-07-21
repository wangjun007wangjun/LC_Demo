using System;
using System.Collections.Generic;
using System.IO;

namespace BaseFramework
{
    public class FileUtil
    {
        /// <summary>
        /// 获取短名字，包括后缀
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetShortName(string path)
        {
            int startIndex = path.LastIndexOf("/", StringComparison.Ordinal);
            if (startIndex > 0)
            {
                path = path.Substring(startIndex + 1, path.Length - startIndex - 1);
            }

            return path;
        }

        /// <summary>
        /// 获取短名字，不包括后缀
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetShortNameNoSuffix(string path)
        {
            path = GetShortName(path);

            int endIndex = path.LastIndexOf(".", StringComparison.Ordinal);
            if (endIndex > 0)
            {
                path = path.Substring(0, endIndex);
            }

            return path;
        }

        /// <summary>
        /// 获取后缀，不包含'.'，eg:"xxx.mp3" return mp3
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSuffix(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            int startIndex = path.LastIndexOf(".", StringComparison.Ordinal);
            if (startIndex > 0)
            {
                path = path.Substring(startIndex + 1, path.Length - startIndex - 1);
            }

            return path;
        }
    
        public static void GetFiles(string path, Dictionary<string, string> result, bool recursion = false)
        {
            if(result == null)
                result = new Dictionary<string, string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            fil.ForEach((index, it) => { result.Add(it.FullName, it.Name);});

            if (recursion)
            {
                DirectoryInfo[] dii = dir.GetDirectories();
                dii.ForEach((index, it) => { GetFiles(it.FullName, result, recursion); });
            }
        }
    }
}
