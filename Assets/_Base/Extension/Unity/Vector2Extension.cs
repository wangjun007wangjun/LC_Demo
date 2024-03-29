﻿using UnityEngine;

namespace BaseFramework
{
    public static class Vector2Extension
    {
        // 缓存一些变量
        private static Vector2 _cacheV2;
        private static Vector3 _cacheV3;

        public static Vector3 ToV3(this Vector2 self, float z = 0)
        {
            _cacheV3.x = self.x;
            _cacheV3.y = self.y;
            _cacheV3.z = z;
            return _cacheV3;
        }

        public static Vector3 XZV3(this Vector2 self, float y = 0)
        {
            _cacheV3.x = self.x;
            _cacheV3.y = y;
            _cacheV3.z = self.y;
            return _cacheV3;
        }

        public static Vector3 YZV3(this Vector2 self, float x = 0)
        {
            _cacheV3.x = x;
            _cacheV3.y = self.x;
            _cacheV3.z = self.y;
            return _cacheV3;
        }

        public static Vector2 NewX(this Vector2 self, float x)
        {
            _cacheV2 = self;
            _cacheV2.x = x;
            return _cacheV2;
        }

        public static Vector2 NewY(this Vector2 self, float y)
        {
            _cacheV2 = self;
            _cacheV2.y = y;
            return _cacheV2;
        }

        public static Vector2 OffsetX(this Vector2 self, float x)
        {
            return self.NewX(self.x + x);
        }

        public static Vector2 OffsetY(this Vector2 self, float y)
        {
            return self.NewY(self.y + y);
        }

        public static Vector2 ScaleX(this Vector2 self, float x)
        {
            return self.NewX(self.x * x);
        }

        public static Vector2 ScaleY(this Vector2 self, float y)
        {
            return self.NewY(self.y * y);
        }

        public static bool IsUniformScale(this Vector2 self)
        {
            return Mathf.Approximately(self.x, self.y);
        }

        /// <summary>
        /// Angle whith right
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float Angle(this Vector2 self)
        {
            return Vector2.right.AngleTo(self);
        }

        public static float AngleTo(this Vector2 from, Vector2 to)
        {
            Vector3 axis = Vector3.Cross(from, to);
            float angle = Vector2.Angle(from, to);
            return axis.z > 0 ? angle : -angle;
        }

        public static Vector2 RotateBy(this Vector2 self, float angle)
        {
            float rad = (self.Angle() + angle) * Mathf.Deg2Rad;
            _cacheV2.x = Mathf.Cos(rad);
            _cacheV2.y = Mathf.Sin(rad);
            return _cacheV2 * self.magnitude;
        }

        public static Vector2 Clamp(this Vector2 self, Rect rect)
        {
            return self.Clamp(rect.min, rect.max);
        }

        public static Vector2 Clamp(this Vector2 self, Vector2 min, Vector2 max)
        {
            _cacheV2.x = Mathf.Clamp(self.x, min.x, max.x);
            _cacheV2.y = Mathf.Clamp(self.y, min.y, max.y);
            return _cacheV2;
        }

        public static Vector2 PositionOnArc(this Vector2 center, float radius, float angle)
        {
            return center + (Vector2.right * radius).RotateBy(angle);
        }

        public static Vector2 RandomPositionOnArc(this Vector2 center, float radius, float startAngle = 0, float endAngle = 360)
        {
            return PositionOnArc(center, radius, Random.Range(startAngle, endAngle));
        }

        /// <summary>
        /// 在扇形区域随机
        /// </summary>
        /// <param name="center"></param>
        /// <param name="maxRadius"></param>
        /// <param name="minRadius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public static Vector2 RandomPositionInSector(this Vector2 center, float maxRadius, float minRadius = 0, float startAngle = 0, float endAngle = 360)
        {
            return RandomPositionOnArc(center, Random.Range(minRadius, maxRadius), startAngle, endAngle);
        }

        public static Transform GetNearest(this Vector2 center, float radius, int layerMask)
        {
            Transform nearest = null;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, layerMask);
            float minDistance = float.MaxValue;
            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(center, collider.transform.position);
                if (!(distance < minDistance)) continue;
                nearest = collider.transform;
                minDistance = distance;
            }
            return nearest;
        }

        public static Transform GetNearestNonAlloc(this Vector2 center, float radius, Collider2D[] colliders, int layerMask)
        {
            Transform nearest = null;
            int count = Physics2D.OverlapCircleNonAlloc(center, radius, colliders, layerMask);
            float minDistance = float.MaxValue;
            for (int i = 0; i < count; ++i)
            {
                float distance = Vector2.Distance(center, colliders[i].transform.position);
                if (!(distance < minDistance)) continue;
                nearest = colliders[i].transform;
                minDistance = distance;
            }
            return nearest;
        }
    }
}