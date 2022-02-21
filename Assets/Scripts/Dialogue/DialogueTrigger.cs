using Assets.Scripts.Constants;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag(Tags.PLAYER))
        {
            DialogueManager.Instance.StartDialogue(this.dialogue);
            this.activated = true;
        }
    }
}
