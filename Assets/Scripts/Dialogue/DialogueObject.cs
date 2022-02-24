using System.Collections;
using UnityEngine;

public abstract class DialogueObject : MonoBehaviour
{
    public string Name;

    private void Start()
    {
        DialogueManager.Instance.RegisterObject(this);
    }

    public abstract IEnumerator Show();

    public abstract IEnumerator Hide();
}
