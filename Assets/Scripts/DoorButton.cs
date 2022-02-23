using Assets.Scripts.Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DoorButton : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Collider2D doorContainer;

    [Header("Effect")]
    [SerializeField] private float tileTiming = 0.8f;
    [SerializeField] private UnityEvent onTileOpened;

    private bool activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated || !collision.CompareTag(Tags.PLAYER))
            return;

        StartCoroutine(OpenDoor());
        this.activated = true;
    }

    private IEnumerator OpenDoor()
    {
        doorContainer.enabled = false;

        foreach (Transform tile in doorContainer.transform)
        {
            tile.gameObject.SetActive(false);
            this.onTileOpened?.Invoke();

            yield return new WaitForSeconds(tileTiming);
        }
    }
}
