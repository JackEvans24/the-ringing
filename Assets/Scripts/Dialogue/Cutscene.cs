using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private Dialogue introDialogue;
    [SerializeField] private bool onStart;

    private bool activated;

    void Start()
    {
        if (this.onStart)
            this.StartCutscene();
    }

    public void StartCutscene()
    {
        if (this.activated)
            return;
        this.activated = true;

        DialogueManager.Instance.StartDialogue(introDialogue);
    }
}
