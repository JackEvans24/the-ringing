using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Vector3 moveDistance;
    [SerializeField] private float moveTime;
    [SerializeField] private Ease moveEasing = Ease.InOutSine;

    private Vector3? startPosition;
    private Tween tween;

    void Start()
    {
        this.startPosition = this.transform.position;

        var targetPosition = this.transform.position + this.moveDistance;
        this.tween = this.transform
            .DOMove(targetPosition, this.moveTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(this.moveEasing);
    }

    private void OnDestroy()
    {
        this.tween.Kill();
    }

    private void OnDrawGizmosSelected()
    {
        var startPos = this.startPosition ?? this.transform.position;
        Gizmos.DrawLine(startPos, startPos + this.moveDistance);
    }
}
