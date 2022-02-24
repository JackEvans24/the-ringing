using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageObject : DialogueObject
{
    [Header("References")]
    [SerializeField] private Image image;

    [Header("Effect")]
    [SerializeField] private float fadeTime = 1f;

    public override IEnumerator Hide()
    {
        yield return this.image.DOFade(0f, this.fadeTime);
    }

    public override IEnumerator Show()
    {
        yield return this.image.DOFade(1f, this.fadeTime);
    }
}
