using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class LayerMaskExtensions
    {
        public static bool CompareLayer(this LayerMask layerMask, int layer) => ((1 << layer) & layerMask.value) != 0;
    }
}
