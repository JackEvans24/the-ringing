using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSmoothing;

    private Vector3 currentVelocity;

    private void FixedUpdate()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, this.target.position, ref currentVelocity, this.followSmoothing);
    }
}
