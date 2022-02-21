using Assets.Scripts.Constants;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fade : MonoBehaviour
{
    private SpriteRenderer sr;
    private ParticleSystem ps;
    private int layer;

    private bool fading;

    [SerializeField] private Collider2D col;

    [SerializeField] private float fadeTime;
    [SerializeField] private Ease fadeEase;
    [SerializeField] private float reappearAfter;
    [SerializeField] private float reappearTime;
    [SerializeField] private Ease reappearEase;

    private void Awake()
    {
        this.sr = GetComponent<SpriteRenderer>();
        this.ps = GetComponentInChildren<ParticleSystem>();
        this.layer = this.gameObject.layer;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.fading)
            return;

        this.fading = true;

        if (ps != null)
            ps.Play();

        var sequence = DOTween.Sequence();
        
        // Fade out and disable collision
        sequence.Append(this.sr.DOFade(0f, this.fadeTime).SetEase(fadeEase).OnComplete(() => DisableCollision()));
        // Wait
        sequence.AppendInterval(this.reappearAfter);
        // Fade in, enabling collision on fade start
        sequence.Append(this.sr.DOFade(1f, this.reappearTime).OnStart(() => this.EnableCollision()).SetEase(reappearEase).OnComplete(() => SequenceComplete()));
    }

    private void DisableCollision()
    {
        this.col.enabled = false;
        this.gameObject.layer = (int)Layers.DEFAULT;
    }

    private void EnableCollision()
    {
        this.col.enabled = true;
        this.gameObject.layer = this.layer;
    }

    private void SequenceComplete()
    {
        this.fading = false;
    }
}
