using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private UnityEvent onCollect;
    [SerializeField] private Dialogue dialogue;

    private bool activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.activated || !collision.CompareTag(Tags.PLAYER))
            return;

        this.activated = true;

        this.onCollect?.Invoke();

        if (this.dialogue != null)
            DialogueManager.Instance.StartDialogue(this.dialogue);
    }
}
