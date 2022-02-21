using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class ProximityHelpers
    {
        public static bool ProximityCircleCheck(IEnumerable<Transform> checks, float radius, LayerMask layer)
        {
            foreach (var check in checks)
            {
                var colliders = Physics2D.OverlapCircleAll(check.position, radius, layer);
                if (colliders.Any())
                    return true;
            }

            return false;
        }

        public static bool ProximityRayCheck(IEnumerable<Transform> checks, Vector2 direction, float distance, LayerMask layer)
        {
            foreach (var check in checks)
            {
                var colliders = Physics2D.RaycastAll(check.position, direction, distance, layer);
                if (colliders.Any())
                    return true;
            }

            return false;
        }
    }
}
