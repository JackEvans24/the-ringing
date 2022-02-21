using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEvent
{
    public string Character;
    [TextArea(1, 4)] public string Text;
}

public enum DialogueEventType
{
    Text
}
