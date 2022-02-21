using Assets.Scripts.Constants;
using System;
using UnityEngine;

namespace Assets.Scripts.Models.Player
{
    public class PlayerCameraTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;

        private bool isFacingLeft;

        public void UpdatePosition(bool isFacingLeft)
        {
            var targetPosition = isFacingLeft ? this.offset * -1 : this.offset;

            if (isFacingLeft == this.isFacingLeft && this.transform.localPosition == targetPosition)
                    return;

            this.isFacingLeft = isFacingLeft;

            this.transform.localPosition = targetPosition;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.ROOM_BOUNDARY))
                CameraController.Instance.SetCurrentCamera(collision.transform);
        }

        public void SetCurrentRoom()
        {
            var boundaryObject = BoundaryManager.GetBoundaryAtPoint(this.transform.position);
            CameraController.Instance.SetCurrentCamera(boundaryObject.transform);
        }
    }
}
