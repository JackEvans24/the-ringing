using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Models.Player
{
    [Serializable]
    public class PlayerHealth
    {
        public int MaxHealth;
        public int CurrentHealth = 1;
        
        [SerializeField] private float invincibilityTime = 0.5f;
        [SerializeField] private HealthBar healthBar;

        [Header("Damage Layers")]
        public LayerMask StandardHitLayer;
        [SerializeField] private LayerMask deathLayer;

        [Header("Events")]
        [SerializeField] private UnityEvent onDeath;
        [SerializeField] private UnityEvent onReset;

        [NonSerialized] public bool IsHurt = false;
        [NonSerialized] public bool IsDead = false;
        [NonSerialized] public Queue<int> DamageQueue = new Queue<int>();
        public bool IsInvincible => this.currentInvincibilityCounter <= this.invincibilityTime;

        private CollisionHelper collisions;

        private bool isInitialised = false;
        private float currentInvincibilityCounter;

        public void Reset(CollisionHelper collisions)
        {
            this.collisions = collisions;

            this.IsDead = false;
            this.onReset?.Invoke();

            this.currentInvincibilityCounter = this.invincibilityTime + 1;
            this.CurrentHealth = this.MaxHealth;

            this.healthBar.ResetHealth(this);

            this.isInitialised = true;
        }

        public void Update()
        {
            if (!this.isInitialised)
                return;

            int? nextDamage = this.DamageQueue.Count > 0 ? this.DamageQueue.Dequeue() : (int?)null;
            this.DamageQueue.Clear();

            if (this.IsInvincible)
            {
                this.currentInvincibilityCounter += Time.deltaTime;
                this.IsHurt = false;
                return;
            }

            if (this.collisions.Hit(this.deathLayer, out _))
                this.TakeDamage(this.CurrentHealth);
            else if (nextDamage.HasValue)
                this.TakeDamage(nextDamage.Value);
            else if (this.collisions.Hit(this.StandardHitLayer, out _))
                this.TakeDamage(1);
        }

        private void TakeDamage(int damage)
        {
            if (!this.isInitialised || this.IsInvincible || this.IsDead)
                return;

            this.CurrentHealth = (int)Math.Max(0f, this.CurrentHealth - damage);
            this.IsHurt = true;

            this.healthBar.UpdateHealth(this);

            if (!this.IsDead && this.CurrentHealth <= 0)
            {
                this.IsDead = true;
                this.Die();
            }
            else
                this.currentInvincibilityCounter = 0f;
        }

        private void Die()
        {
            this.onDeath?.Invoke();
        }
    }
}
