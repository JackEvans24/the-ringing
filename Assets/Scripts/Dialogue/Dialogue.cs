using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public bool Passive = false;
    public float PassiveTimeout = 2f;
    public List<DialogueEvent> Events;
}
