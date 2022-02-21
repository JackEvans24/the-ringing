using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Boundary : MonoBehaviour
{
    [HideInInspector] public PolygonCollider2D Collider;

    void Awake()
    {
        this.Collider = GetComponent<PolygonCollider2D>();
        BoundaryManager.RegisterBoundary(this);
    }
}
