using UnityEngine;

[System.Serializable]
public class LookAheadController
{
    [SerializeField] private Transform lookAheadPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float dampingDuration;

    private bool isFacingLeft;
    private float timeSinceDirectionChange;
    private Vector3 oldPosition;

    public void UpdatePosition(bool isFacingLeft)
    {
        var targetPosition = isFacingLeft ? this.offset * -1 : this.offset;

        if (isFacingLeft == this.isFacingLeft)
        {
            if (this.lookAheadPoint.localPosition == targetPosition)
                return;

            this.timeSinceDirectionChange += Time.deltaTime;
        }
        else
        {
            this.isFacingLeft = isFacingLeft;
            this.timeSinceDirectionChange = 0f;
            this.oldPosition = this.lookAheadPoint.localPosition;
        }

        this.lookAheadPoint.localPosition = Vector3.Lerp(this.oldPosition, targetPosition, this.timeSinceDirectionChange / dampingDuration);
    }
}
