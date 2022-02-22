using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;

public class FallingSpike : MonoBehaviour
{
    [SerializeField] private LayerMask destructionLayers;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float destroyAfter = 2f;
    [SerializeField] private UnityEvent onDestroy;

    private void FixedUpdate()
    {
        this.transform.Translate(Vector2.down * this.speed * 0.01f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.destructionLayers.CompareLayer(collision.gameObject.layer))
            return;

        Destroy(this.gameObject, this.destroyAfter);
        this.onDestroy?.Invoke();
    }
}
