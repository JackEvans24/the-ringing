using Assets.Scripts.Constants;
using UnityEngine;

public class UpdateRingTarget : MonoBehaviour
{
    [SerializeField] private Transform newTarget;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag(Tags.PLAYER))
        {
            RingingManager.SetRingingPosition(this.newTarget);
            this.activated = true;
        }
    }
}
