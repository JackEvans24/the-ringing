using Assets.Scripts.Helpers;
using Assets.Scripts.Tweening;
using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private Collider2D collider;
    private ParticleSystem deathParticles;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    public float speed;
    public Vector2 direction;

    public bool Wobble;
    private Sequence wobbleSequence;
    [SerializeField] private Vector2 wobbleScale;
    [SerializeField] private float wobbleSpeed;

    [SerializeField] private LayerMask killLayers;
    [SerializeField] private float killAfter;

    private bool IsInactive;

    private void Awake()
    {
        this.collider = GetComponent<Collider2D>();
        this.deathParticles = GetComponent<ParticleSystem>();
        this.rb = GetComponent<Rigidbody2D>();
        this.sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (this.Wobble && this.wobbleSequence == null)
            this.StartWobble();
        else if (!this.Wobble && this.wobbleSequence != null)
            this.StopWobble();

        this.rb.velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (killLayers.CompareLayer(collision.gameObject.layer))
            this.Kill();
    }

    private void Kill()
    {
        if (this.IsInactive)
            return;

        this.IsInactive = true;

        this.collider.enabled = false;
        this.sprite.enabled = false;
        this.StopWobble();
        this.deathParticles.Play();

        this.speed = 0;

        Destroy(this.gameObject, this.killAfter);
    }

    private void StartWobble()
    {
        if (this.IsInactive)
            return;

        this.wobbleSequence = TweenHelpers.Wobble(this.transform, this.wobbleScale, this.wobbleSpeed);
        this.wobbleSequence.Play();
    }

    private void StopWobble()
    {
        this.wobbleSequence.Kill();
        this.wobbleSequence = null;
    }
}
