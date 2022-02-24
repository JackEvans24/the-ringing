using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance { get => instance; }


    [Header("References")]
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text promptText;

    [Header("Variables")]
    [SerializeField] private float characterWaitTime = 0.05f;
    [SerializeField] private float dialogFadeTime = 0.1f;

    [Header("Events")]
    [SerializeField] private UnityEvent onShowCharacter;
    [SerializeField] private UnityEvent onLevelEnd;

    private PlayerController player;
    private Dialogue currentDialogue;
    private DialogueEvent currentEvent;
    private Queue<Dialogue> awaitingDialogues = new Queue<Dialogue>();
    private Queue<DialogueEvent> awaitingEvents = new Queue<DialogueEvent>();
    private Dictionary<string, DialogueObject> dialogueObjects = new Dictionary<string, DialogueObject>();

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

        DontDestroyOnLoad(this.gameObject);

        this.Reset();
    }

    private void Reset()
    {
        this.player = FindObjectOfType<PlayerController>();
        this.currentDialogue = null;
        this.currentEvent = null;
        this.awaitingDialogues.Clear();
        this.awaitingEvents.Clear();
        this.dialogueObjects.Clear();

        this.playerCouldMove = false;
        this.writing = false;
        this.skipWriting = false;
        this.isModalOpen = false;
    }

    private void Update()
    {
        if (this.currentDialogue?.Passive != false)
            return;
        if (this.currentEvent?.Type != DialogueEventType.Text)
            return;

        var inputs = this.player.InputConverter.CurrentInputs;
        if (inputs.StartJump)
        {
            if (writing)
                skipWriting = true;
            else
                StartCoroutine(NextEvent());
        }
    }

    public void RegisterObject(DialogueObject dialogueObject)
    {
        if (this.dialogueObjects.ContainsKey(dialogueObject.Name))
            return;

        this.dialogueObjects.Add(dialogueObject.Name, dialogueObject);
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

        if (!this.currentDialogue.Passive && this.player != null)
        {
            this.playerCouldMove = this.player.CanMove;
            this.player.CanMove = false;
        }

        this.promptText.alpha = this.currentDialogue.Passive ? 0 : 1;

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
        this.currentEvent = ev;

        switch (ev.Type)
        {
            case DialogueEventType.Text:
                yield return ShowText(ev);
                break;

            case DialogueEventType.EndOfLevel:
                yield return this.EndLevel(ev);
                break;

            case DialogueEventType.HideBubble:
                yield return this.CloseDialogBox();
                break;

            case DialogueEventType.ShowObject:
                yield return this.ShowObject(ev);
                break;

            case DialogueEventType.HideObject:
                yield return this.HideObject(ev);
                break;

            case DialogueEventType.Ring:
                RingingManager.Ring();
                yield return AwaitTimeout(ev);
                break;

            case DialogueEventType.Wait:
                yield return AwaitTimeout(ev);
                break;

        }

        if (this.currentDialogue.Passive)
            yield return new WaitForSeconds(this.currentDialogue.PassiveTimeout);
        else if (ev.Type == DialogueEventType.Text)
            yield break;

        StartCoroutine(NextEvent());
    }

    private IEnumerator ShowText(DialogueEvent ev)
    {
        this.nameText.text = ev.Character;
        yield return ShowLineOfText(ev.Text);
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
        this.currentEvent = null;

        StartCoroutine(this.NextDialog());
    }

    private IEnumerator EndLevel(DialogueEvent ev)
    {
        this.onLevelEnd?.Invoke();

        yield return AwaitTimeout(ev);
        SceneManager.LoadScene((int)ev.NextScene);
    }

    private IEnumerator AwaitTimeout(DialogueEvent ev)
    {
        yield return new WaitForSeconds(ev.Timeout);
    }

    private IEnumerator ShowObject(DialogueEvent ev)
    {
        if (!this.dialogueObjects.TryGetValue(ev.DialogueObjectName, out var obj))
            yield break;

        yield return obj.Show();
    }

    private IEnumerator HideObject(DialogueEvent ev)
    {
        if (!this.dialogueObjects.TryGetValue(ev.DialogueObjectName, out var obj))
            yield break;

        yield return obj.Hide();
    }
}
