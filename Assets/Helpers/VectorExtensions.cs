using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Helpers
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2xz(this Vector3 v)
        {
            return new Vector3(v.x, v.z);
        }

        public static Vector3 ToVector3xz(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static Vector3 ToVector3xz(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static Vector3 TriangleNormal(this Vector3 x, Vector3 y, Vector3 z)
        {
            return Vector3.Cross(x-y, y-z);
        }

        public static float Area(this Vector3 a, Vector3 b, Vector3 c)
        {
            //brnkhy - this looks retarded
            return ((a.x - c.x) * (b.z - c.z) - (a.z - c.z) * (b.x - c.x)) / 2;
        }

        public static Vector3 ToPixel(this Vector2 hc)
        {
            var x = (hc.x * Globals.Width) + (((int)hc.y & 1) * Globals.Width / 2);
            return new Vector3(x, 0, (float)(hc.y * 1.5 * Globals.Radius));
        }

        

    }
}
