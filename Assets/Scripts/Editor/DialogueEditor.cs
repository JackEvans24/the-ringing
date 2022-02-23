using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var dialogue = (Dialogue)target;

        this.DrawPassiveToggle(dialogue);

        this.DrawEventsList(dialogue);

        this.DrawAddButtons(dialogue);
    }

    private void DrawPassiveToggle(Dialogue dialogue)
    {
        GUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Dialogue.Passive)));

        if (dialogue.Passive)
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Dialogue.PassiveTimeout)));

        GUILayout.EndHorizontal();
    }

    private void DrawAddButtons(Dialogue dialogue)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add text event"))
            this.AddEvent(dialogue, DialogueEventType.Text);

        if (GUILayout.Button("Add level end event"))
            this.AddEvent(dialogue, DialogueEventType.EndOfLevel);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset dialogue"))
            if (EditorUtility.DisplayDialog("Reset?", "Are you sure you want to reset dialogue? This will delete all data from this object", "hehe ye", "no go back"))
                this.ResetDialog(dialogue);
    }

    private void AddEvent(Dialogue dialogue, DialogueEventType eventType)
    {
        if (dialogue.Events == null)
            dialogue.Events = new List<DialogueEvent>();

        dialogue.Events.Add(new DialogueEvent() { Type = eventType });

        serializedObject.Update();
    }

    private void ResetDialog(Dialogue dialogue)
    {
        dialogue.Events = new List<DialogueEvent>();
        serializedObject.Update();
    }

    private void DrawEventsList(Dialogue dialogue)
    {
        var events = serializedObject.FindProperty(nameof(Dialogue.Events));

        for (int i = 0; i < events.arraySize; i++)
        {
            if (!DrawEventView(dialogue, dialogue.Events.ElementAt(i), events.GetArrayElementAtIndex(i)))
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool DrawEventView(Dialogue dialogue, DialogueEvent ev, SerializedProperty prop)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("↑", GUILayout.Width(25)))
        {
            dialogue.Events.MovePrevious(ev);
            serializedObject.Update();
            return false;
        }

        EditorGUILayout.PropertyField(prop.FindPropertyRelative(nameof(DialogueEvent.Type)));

        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            dialogue.Events.Remove(ev);
            serializedObject.Update();
            return false;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("↓", GUILayout.Width(25)))
        {
            dialogue.Events.MoveNext(ev);
            serializedObject.Update();
            return false;
        }

        switch (ev.Type)
        {
            case DialogueEventType.Text:
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(nameof(DialogueEvent.Text)));
                break;
            case DialogueEventType.EndOfLevel:
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(nameof(DialogueEvent.NextScene)));
                break;
        }

        GUILayout.EndHorizontal();

        if (ev.Type == DialogueEventType.EndOfLevel)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(28);
            EditorGUILayout.PropertyField(prop.FindPropertyRelative(nameof(DialogueEvent.NextSceneTimer)));

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        return true;
    }
}
