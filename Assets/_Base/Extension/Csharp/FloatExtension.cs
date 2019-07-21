using UnityEngine;

namespace BaseFramework
{
    public static class FloatExtension
    {
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Eq(this float self, float target)
        {
            return Mathf.Abs(self - target) < Mathf.Epsilon;
        }

        /// <summary>
        /// 不等
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Ne(this float self, float target)
        {
            return !Eq(self, target);
        }

        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Lt(this float self, float target)
        {
            return !Eq(self, target) && self < target;
        }

        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Gt(this float self, float target)
        {
            return !Eq(self, target) && self > target;
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Le(this float self, float target)
        {
            return !Gt(self, target);
        }

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Ge(this float self, float target)
        {
            return !Lt(self, target);
        }
    }
}
