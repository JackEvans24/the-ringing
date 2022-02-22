using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance { get => instance; }

    [Header("References")]
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Variables")]
    [SerializeField] private float characterWaitTime = 0.05f;
    [SerializeField] private float dialogFadeTime = 0.1f;

    [Header("Events")]
    [SerializeField] private UnityEvent onShowCharacter;

    private PlayerController player;
    private Dialogue currentDialogue;
    private Queue<Dialogue> awaitingDialogues = new Queue<Dialogue>();
    private Queue<DialogueEvent> awaitingEvents = new Queue<DialogueEvent>();

    private bool playerCouldMove;
    private bool writing, skipWriting;
    private bool isModalOpen;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        this.player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        var inputs = this.player.InputConverter.CurrentInputs;
        if (inputs.Action)
        {
            if (writing)
                skipWriting = true;
            else
                StartCoroutine(NextEvent());
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        this.awaitingDialogues.Enqueue(dialogue);
        StartCoroutine(NextDialog());
    }

    private IEnumerator NextDialog()
    {
        if (this.currentDialogue != null)
            yield break;

        if (this.awaitingDialogues.Count == 0)
        {
            yield return CloseDialogBox();
            yield break;
        }

        this.currentDialogue = this.awaitingDialogues.Dequeue();

        if (!this.currentDialogue.Passive)
        {
            this.playerCouldMove = this.player.CanMove;
            this.player.CanMove = false;
        }

        this.awaitingEvents.Clear();
        foreach (var ev in this.currentDialogue.Events)
            this.awaitingEvents.Enqueue(ev);

        StartCoroutine(NextEvent());
    }

    private IEnumerator NextEvent()
    {
        if (awaitingEvents.Count == 0)
        {
            EndDialog();
            yield break;
        }

        skipWriting = false;

        yield return new WaitForSeconds(0.05f);

        var ev = awaitingEvents.Dequeue();
        this.nameText.text = ev.Character;

        yield return ShowLineOfText(ev.Text);

        if (!this.currentDialogue.Passive)
            yield break;

        yield return new WaitForSeconds(this.currentDialogue.PassiveTimeout);
        StartCoroutine(NextEvent());
    }

    private IEnumerator ShowLineOfText(string line)
    {
        this.writing = true;

        if (!this.isModalOpen)
            yield return OpenDialogBox();

        var visibleCount = 0;

        this.dialogueText.maxVisibleCharacters = visibleCount;
        this.dialogueText.text = line;

        while (true)
        {
            if (this.skipWriting)
            {
                visibleCount = line.Length;
                this.dialogueText.maxVisibleCharacters = visibleCount;
            }

            if (visibleCount >= line.Length)
            {
                this.writing = false;
                yield break;
            }

            visibleCount++;
            this.dialogueText.maxVisibleCharacters = visibleCount;

            this.onShowCharacter?.Invoke();

            yield return new WaitForSeconds(characterWaitTime);
        }
    }

    private IEnumerator OpenDialogBox()
    {
        this.dialogueText.text = string.Empty;
        this.canvas.DOFade(1, dialogFadeTime);

        this.isModalOpen = true;

        yield return new WaitForSeconds(dialogFadeTime);
    }

    private IEnumerator CloseDialogBox()
    {
        dialogueText.text = string.Empty;
        canvas.DOFade(0, dialogFadeTime);

        this.isModalOpen = false;

        yield return new WaitForSeconds(dialogFadeTime);
    }

    private void EndDialog()
    {
        if (!this.currentDialogue.Passive)
            player.CanMove = this.playerCouldMove;

        this.currentDialogue = null;

        StartCoroutine(this.NextDialog());
    }
}
