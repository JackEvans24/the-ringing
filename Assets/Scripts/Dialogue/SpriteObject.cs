using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SpriteObject : DialogueObject
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;

    [Header("Effect")]
    [SerializeField] private float fadeTime = 1f;

    public override IEnumerator Hide()
    {
        yield return this.sprite.DOFade(0f, this.fadeTime);
    }

    public override IEnumerator Show()
    {
        yield return this.sprite.DOFade(1f, this.fadeTime);
    }
}
