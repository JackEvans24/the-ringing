using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CanvasObject : DialogueObject
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvas;

    [Header("Effect")]
    [SerializeField] private float fadeTime = 1f;

    public override IEnumerator Hide()
    {
        yield return this.canvas.DOFade(0f, this.fadeTime);
    }

    public override IEnumerator Show()
    {
        yield return this.canvas.DOFade(1f, this.fadeTime);
    }
}
