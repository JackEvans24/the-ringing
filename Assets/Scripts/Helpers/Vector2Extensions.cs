using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Vector2Extensions
    {
        public static Vector2 Invert(this Vector2 v) => new Vector2(v.y, v.x);
        public static Vector3 ToVector3(this Vector2 v, float z) => new Vector3(v.x, v.y, z);
    }
}
