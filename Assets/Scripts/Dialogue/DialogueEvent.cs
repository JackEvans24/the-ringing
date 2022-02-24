using Assets.Scripts.Constants;
using UnityEngine;

[System.Serializable]
public class DialogueEvent
{
    public DialogueEventType Type;
    public string Character;
    [TextArea(1, 4)] public string Text;
    public Scenes NextScene;
    public float Timeout;
    public string DialogueObjectName;
}

public enum DialogueEventType
{
    Text,
    EndOfLevel,
    ShowObject,
    HideObject,
    Ring,
    Wait,
    HideBubble
}
