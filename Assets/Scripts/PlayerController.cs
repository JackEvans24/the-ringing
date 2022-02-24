using Assets.Scripts.Constants;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [HideInInspector] public PlayerInputConverter InputConverter;

    [SerializeField] private bool cutsceneMode;
    [NonSerialized] public bool CanMove;

    [SerializeField] private CollisionHelper collisions;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Fallthrough fallthrough;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerColour colour;
    [SerializeField] private LookAheadController lookAhead;
    [SerializeField] private PlayerCameraTrigger cameraTrigger;
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private float deathResetDelay = 2f;
    private bool resetting = false;

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();

        this.movement.Collisions = this.collisions;

        var inputSystem = GetComponent<PlayerInput>();
        this.InputConverter = new PlayerInputConverter();
        this.InputConverter.PerformSetup(inputSystem);
    }

    private void Start()
    {
        if (this.cutsceneMode)
            return;

        this.Reset();
    }

    void Update()
    {
        // Update health
        this.health.Update();
        if (this.health.IsDead)
            StartCoroutine(this.ResetAfterDelay());

        // Set movement ability
        this.movement.CanMove = this.CanMove;

        // Get movement variables
        this.InputConverter.UpdateInputs();
        var inputs = this.InputConverter.CurrentInputs;

        // Set fallthrough
        this.fallthrough.Update(this.gameObject, inputs.Fallthrough);
        inputs.Fallthrough |= this.fallthrough.ShouldFall;

        // Set movement variables
        this.movement.Update(inputs, this.health.IsHurt, this.health.IsDead);
        this.movement.UpdateSprite(this.sprite);

        // Update colour
        if (this.colour.Enabled)
            this.sprite.color = this.colour.GetPlayerColour(this.movement.isDashing, this.health.IsInvincible, this.health.IsDead);
    }

    void FixedUpdate()
    {
        this.rb.velocity = new Vector2(movement.currentHorizontalVelocity, movement.currentVerticalVelocity);
    }

    void LateUpdate()
    {
        // Update look ahead
        this.lookAhead.UpdatePosition(this.movement.IsFacingLeft);
        this.cameraTrigger.UpdatePosition(this.movement.IsFacingLeft);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.health.StandardHitLayer.CompareLayer(collision.gameObject.layer))
            this.health.DamageQueue.Enqueue(1);
    }

    private IEnumerator ResetAfterDelay()
    {
        if (resetting) yield break;
        this.resetting = true;

        yield return new WaitForSeconds(deathResetDelay);

        this.Reset();

        this.resetting = false;
    }

    private void Reset()
    {
        this.transform.position = CheckpointManager.Instance.CurrentCheckpoint.transform.position;
        this.CanMove = true;

        this.movement.Reset();
        this.health.Reset(this.collisions);

        this.cameraTrigger.SetCurrentRoom();
    }

    void OnDrawGizmosSelected()
    {
        this.collisions.DrawGizmos();
    }
}
