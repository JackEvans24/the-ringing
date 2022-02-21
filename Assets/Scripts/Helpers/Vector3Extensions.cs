using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Vector3Extensions
    {
        public static Vector3 New(this Vector3 v) => new Vector3(v.x, v.y, v.z);
    }
}
