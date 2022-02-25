using System.Collections;
using UnityEngine;

public class ScriptObject : DialogueObject
{
    [SerializeField] private MonoBehaviour script;

    public override IEnumerator Hide()
    {
        script.enabled = false;
        yield return null;
    }

    public override IEnumerator Show()
    {
        script.enabled = true;
        yield return null;
    }
}
