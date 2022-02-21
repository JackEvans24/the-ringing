using DG.Tweening;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private Vector3 moveDistance;
    [SerializeField] private float moveDuration;
    [SerializeField] private Ease ease;

    private Tween tween;
    private Vector3 startPos;

    private void Awake()
    {
        if (this.tween != default)
            return;

        this.startPos = this.transform.position;
        var targetPos = this.startPos + moveDistance;

        this.tween = this.transform.DOMove(targetPos, moveDuration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDrawGizmosSelected()
    {
        var startPos = this.startPos == null ? this.transform.position : this.startPos;
        var targetPos = startPos + moveDistance;

        Gizmos.DrawLine(startPos, targetPos);
    }
}
