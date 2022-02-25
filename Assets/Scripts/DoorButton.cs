using Assets.Scripts.Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DoorButton : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Collider2D[] doorContainers;

    [Header("Effect")]
    [SerializeField] private float tileTiming = 0.8f;
    [SerializeField] private UnityEvent onTileOpened;

    private bool activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.activated || !collision.CompareTag(Tags.PLAYER))
            return;

        this.activated = true;
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        foreach (var door in this.doorContainers)
        {
            door.enabled = false;

            foreach (Transform tile in door.transform)
            {
                if (!tile.gameObject.activeInHierarchy)
                    continue;

                tile.gameObject.SetActive(false);
                this.onTileOpened?.Invoke();

                yield return new WaitForSeconds(tileTiming);
            }
        }
    }
}
