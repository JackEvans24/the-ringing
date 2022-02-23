using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.Events;

public class Crystal : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private UnityEvent onTouch;

    private bool activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated || !collision.CompareTag(Tags.PLAYER))
            return;

        this.activated = true;
        CrystalActivated();
    }

    private void CrystalActivated()
    {
        this.onTouch?.Invoke();

        RingingManager.EndOfLevel(this.transform);
        DialogueManager.Instance.StartDialogue(this.dialogue);
    }
}
