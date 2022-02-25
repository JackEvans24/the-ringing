using Assets.Scripts.Helpers;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models.Player
{
    [Serializable]
    public class PlayerMovement
    {
        [NonSerialized] public CollisionHelper Collisions;
        [NonSerialized] public bool CanMove;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("Velocities")]
        [SerializeField] private float horizontalVelocity = 5f;
        [SerializeField] private float upwardVelocity = 6f;
        [SerializeField] private float downwardVelocity = 8f;

        [NonSerialized] public float currentHorizontalVelocity;
        [NonSerialized] public float currentVerticalVelocity;

        [Header("Wall Checks")]
        [SerializeField] private LayerMask wallLayer;

        [Header("Jumping")]
        [SerializeField] private float jumpDuration = 0.4f;
        [SerializeField] private AnimationCurve jumpCurve;
        [SerializeField] private float terminalVelocityTime = 1f;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpInputLag = 0.1f;

        private float timeSinceJumpInput;
        private float timeSpentFalling;

        private bool isJumping;
        private float currentJumpDuration;
        private float currentCoyoteTime;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask fallthroughLayer;

        private bool isGrounded;

        [Header("Ceiling Check")]
        [SerializeField] private LayerMask ceilingLayer;
        private bool ceilingHit;

        // Facing
        [NonSerialized] public bool IsFacingLeft;
        private bool spriteIsFacingLeft;

        [Header("Recoil")]
        [SerializeField] private float recoilTime = 1f;
        [SerializeField] private Vector2 recoilVelocity;
        [SerializeField] private AnimationCurve recoilCurve;
        private bool isHurt;
        private bool isDead;
        private float currentRecoilTime;

        [Header("Dash")]
        [SerializeField] public bool DashEnabled = true;
        [SerializeField] private float dashVelocity = 10f;
        [SerializeField] private float dashDuration = 0.8f;
        [SerializeField] private AnimationCurve dashCurve;

        [NonSerialized] public bool isDashing;
        private float currentDashTime;

        public void Reset()
        {
            this.currentRecoilTime = this.recoilTime + 1f;
            this.currentDashTime = this.dashDuration + 1f;
        }

        public void Update(PlayerInputConverter.Inputs inputs, bool isHurt, bool isDead)
        {
            // Check ground
            this.CheckGrounded(inputs.Fallthrough);

            if (this.isGrounded && this.currentVerticalVelocity <= 0)
                this.currentJumpDuration = 0;
            else if (isJumping)
                this.currentJumpDuration += Time.deltaTime;

            // Check ceiling
            this.CheckCeiling();

            // Check if should be recoiling
            if (this.ShouldRecoil(isHurt))
                return;

            // Check dash
            this.isDead = isDead;
            this.CheckDash(inputs.Dash);

            // Check dead
            if (this.isDead)
            {
                this.currentHorizontalVelocity = this.CalculateDeadHorizontalVelocity();
                this.currentVerticalVelocity = this.CalculateDeadVerticalVelocity();
                return;
            }

            if (this.CanMove)
            {
                // If the player can move, set velocities
                this.currentHorizontalVelocity = this.CalculateHorizontalVelocity(inputs.Movement);
                this.currentVerticalVelocity = this.CalculateVerticalVelocity(inputs.StartJump, inputs.SustainJump);
            }
            else
            {
                // Otherwise, make sure the player can still fall
                this.currentHorizontalVelocity = 0f;
                this.currentVerticalVelocity = this.CalculateVerticalVelocity(false, false);
            }

            // Set facing direction
            if (inputs.Movement < 0)
                this.IsFacingLeft = true;
            else if (inputs.Movement > 0)
                this.IsFacingLeft = false;
        }

        public void UpdateSprite(SpriteRenderer sprite)
        {
            this.animator.SetFloat("Speed", Mathf.Abs(this.currentHorizontalVelocity));
            this.animator.SetBool("Grounded", this.isGrounded);

            if (this.IsFacingLeft == this.spriteIsFacingLeft)
                return;

            sprite.transform.Rotate(Vector3.up * 180f);
            this.spriteIsFacingLeft = this.IsFacingLeft;
        }

        private void CheckGrounded(bool fallthrough)
        {
            // Set layers to check
            var layers = (int)this.groundLayer;
            if (!fallthrough)
                layers |= (int)this.fallthroughLayer;

            // Check if the player is on the floor
            this.isGrounded = this.Collisions.HitBottom(layers, out _);

            // Keep note of how long since the player left the floor, so they can jump slightly after they run off a platform
            if (!this.isGrounded)
                this.currentCoyoteTime += Time.deltaTime;
            else
                this.currentCoyoteTime = 0f;

            // Reset dash if necessary
            if (isGrounded && !isDashing && this.currentDashTime != 0f)
                this.currentDashTime = 0f;
        }

        private void CheckCeiling()
        {
            this.ceilingHit = this.Collisions.HitTop(this.ceilingLayer, out _);
        }

        private bool ShouldRecoil(bool isHurt)
        {
            // If hurt flag changed, set it
            if (isHurt != this.isHurt)
            {
                this.isHurt = isHurt;

                // If player is hurt this frame, reset dash and recoil time
                if (this.isHurt)
                {
                    this.isDashing = false;
                    this.currentDashTime = 0f;

                    this.currentRecoilTime = 0f;
                }
            }

            // Return false if player is finished recoiling
            if (this.currentRecoilTime > this.recoilTime)
                return false;
            
            // Update recoil time
            this.currentRecoilTime += Time.deltaTime;

            // Set recoil speeds
            var directionCoefficient = this.IsFacingLeft ? 1 : -1;
            this.currentHorizontalVelocity = this.recoilVelocity.x * directionCoefficient * recoilCurve.Evaluate(this.currentRecoilTime / this.recoilTime);
            this.currentVerticalVelocity = this.recoilVelocity.y * recoilCurve.Evaluate(this.currentRecoilTime / this.recoilTime);

            return true;
        }

        private void CheckDash(bool dash)
        {
            // If dashing not enabled, return
            if (!this.DashEnabled || !this.CanMove || this.isDead)
            {
                this.isDashing = false;
                this.currentDashTime = 0f;
                return;
            }

            // Check if the player can start dashing
            if (dash && currentDashTime == 0f)
                this.isDashing = true;

            // Progress current dash time if dashing
            if (this.isDashing)
                this.currentDashTime += Time.deltaTime;

            // End dash if current dash finished
            if (this.currentDashTime >= this.dashDuration)
                this.isDashing = false;
        }

        private float CalculateHorizontalVelocity(float horizontal)
        {
            // Check if the user is blocked in the direction they're trying to travel in
            var blockedRight = horizontal > 0 && this.Collisions.HitRight(this.wallLayer, out _);
            var blockedLeft = horizontal < 0 && this.Collisions.HitLeft(this.wallLayer, out _);
            if (blockedLeft || blockedRight) return 0f;

            // Return dash speed if dashing
            if (this.isDashing)
            {
                var directionCoefficient = this.IsFacingLeft ? -1 : 1;
                return this.dashVelocity * dashCurve.Evaluate(this.currentDashTime / this.dashDuration) * directionCoefficient;
            }

            // Return horizontal speed
            return horizontal * this.horizontalVelocity;
        }

        private float CalculateVerticalVelocity(bool startJump, bool sustainJump)
        {
            // Keep tabs on how long since we hit jump, so if the player hits jump slightly before they're on the ground they're forgiven
            if (startJump)
                this.timeSinceJumpInput = 0f;
            else if (this.timeSinceJumpInput < this.jumpInputLag)
                this.timeSinceJumpInput += Time.deltaTime;

            // Return 0 if dashing
            if (this.isDashing)
                return 0f;

            // Don't let the user keep jumping if they let go of jump
            if (this.isJumping && !sustainJump)
                this.currentJumpDuration = this.jumpDuration + 1;

            // Check if the user can start or sustain their jump
            var canStartJump = this.isGrounded || (!this.isJumping && this.currentCoyoteTime <= this.coyoteTime);
            var canSustainJump = this.isJumping && this.currentJumpDuration <= this.jumpDuration;

            // Check if the user should be going up
            this.isJumping = !this.ceilingHit && ((canStartJump && (startJump || this.timeSinceJumpInput < this.jumpInputLag)) || (canSustainJump && sustainJump));

            if (this.isJumping || this.isGrounded)
                this.timeSpentFalling = 0f;
            else
            {
                this.timeSpentFalling += Time.deltaTime;
            }

            // Return relevant vertical velocity
            if (this.isJumping)
                return this.upwardVelocity * this.jumpCurve.Evaluate(this.currentJumpDuration / this.jumpDuration);
            else if (this.isGrounded)
                return 0f;
            else
                return this.GetDownwardVelocity();
        }

        private float CalculateDeadHorizontalVelocity()
        {
            return 0f;
        }

        private float CalculateDeadVerticalVelocity()
        {
            if (this.isGrounded)
                return 0f;
            else
            {
                this.timeSpentFalling += Time.deltaTime;
                return this.GetDownwardVelocity();
            }
        }

        private float GetDownwardVelocity() => Mathf.Lerp(this.currentVerticalVelocity, this.downwardVelocity * -1, this.timeSpentFalling / this.terminalVelocityTime);
    }
}
