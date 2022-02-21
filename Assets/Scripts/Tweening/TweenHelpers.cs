using Assets.Scripts.Helpers;
using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tweening
{
    public static class TweenHelpers
    {
        public static Sequence Wobble(Transform transform, Vector2 wobble, float time, int loops = -1)
        {
            var sequence = DOTween.Sequence();
            var originalScale = transform.localScale.New();

            sequence.Append(transform.DOScale(wobble.ToVector3(originalScale.z), time / 4));
            sequence.Append(transform.DOScale(wobble.Invert().ToVector3(originalScale.z), time / 2));
            sequence.Append(transform.DOScale(originalScale, time / 4));

            return sequence.SetLoops(loops);
        }
    }
}
