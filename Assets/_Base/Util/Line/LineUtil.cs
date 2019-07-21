using System;
using UnityEngine;

namespace BaseFramework
{
    public class LineUtil
    {
        public static Vector3[] GetSmoothPoints(Vector3[] points, int smooth = 10)
        {
            if (points.Length < 3)
                return points;

            Vector3 startPoint = points[0];

            int smoothAmount = points.Length * smooth;
            points = PathControlPointGenerator(points);
            Vector3[] result = new Vector3[smoothAmount];
            for (int i = 1; i <= smoothAmount; i++)
            {
                float pm = (float)i / smoothAmount;
                Vector3 currPt = Interp(points, pm);
                result[i - 1] = currPt;
            }

            result[0] = startPoint;

            return result;
        }

        private static Vector3[] PathControlPointGenerator(Vector3[] path)
        {
            //create and store path points:
            Vector3[] suppliedPath = path;

            //populate calculate path;
            int offset = 2;
            Vector3[] vector3S = new Vector3[suppliedPath.Length + offset];
            Array.Copy(suppliedPath, 0, vector3S, 1, suppliedPath.Length);

            //populate start and end control points:
            //vector3s[0] = vector3s[1] - vector3s[2];
            vector3S[0] = vector3S[1] + (vector3S[1] - vector3S[2]);
            vector3S[vector3S.Length - 1] = vector3S[vector3S.Length - 2] + (vector3S[vector3S.Length - 2] - vector3S[vector3S.Length - 3]);

            //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
            if (vector3S[1] == vector3S[vector3S.Length - 2])
            {
                Vector3[] tmpLoopSpline = new Vector3[vector3S.Length];
                Array.Copy(vector3S, tmpLoopSpline, vector3S.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3S = new Vector3[tmpLoopSpline.Length];
                Array.Copy(tmpLoopSpline, vector3S, tmpLoopSpline.Length);
            }

            return (vector3S);
        }

        private static Vector3 Interp(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
            float u = t * numSections - currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
            );
        }
        
        /// <summary>
        /// 根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t"></param>T值
        /// <param name="p0"></param>起始点
        /// <param name="p1"></param>控制点
        /// <param name="p2"></param>目标点
        /// <returns></returns>根据T值计算出来的贝赛尔曲线点
        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        /// <summary>
        /// 获取存储贝塞尔曲线点的数组
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="controlPoint"></param>控制点
        /// <param name="endPoint"></param>目标点
        /// <param name="segmentNum"></param>采样点的数量
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static Vector3[] GetBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum) {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++) {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                                                          controlPoint, endPoint);
                path[i - 1] = pixel;
            }
            return path;

        }
    }
}
