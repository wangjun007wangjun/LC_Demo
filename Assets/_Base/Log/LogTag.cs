using System;

namespace BaseFramework
{
    public static class LogTag
    {
        public static string GetLogTag(this object obj, string format = "[{0}]", params object[] args)
        {
            while (true)
            {
                if (obj.IsNull())
                {
                    obj = "null";
                    continue;
                }

                Type type = obj.GetType();
                if (type.IsTypeof<string>())
                {
                    return format.Format(obj);
                }
                if (type.IsSimple())
                {
                    obj = obj.ToString();
                    continue;
                }

                obj = type.ReadableName();
            }
        }
    }
}
