using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models.Player
{
    [Serializable]
    public class PlayerColour
    {
        public bool Enabled;

        [Header("Default")]
        [SerializeField] private Color defaultColour;

        [Header("Hurt")]
        [Range(0, 1)]
        [SerializeField] private float invincibleAlpha = 0.5f;
        [SerializeField] private float invincibleFlashDuration = 0.02f;
        [SerializeField] private Color deadColour;

        private bool isInvincible;
        private float currentInvincibleFlash;

        [Header("Dash")]
        [SerializeField] private Color dashColour;

        public Color GetPlayerColour(bool isDashing, bool isInvincible, bool isDead)
        {
            if (!this.Enabled)
                throw new Exception("Colour class not enabled. Don't call this class without checking if it is enabled");

            this.HandleInvincibilityTimings(isInvincible);

            var color = this.GetCurrentColor(isDashing, isDead);
            color.a = this.GetCurrentAlpha(isDead);

            return color;
        }

        private void HandleInvincibilityTimings(bool isInvincible)
        {
            if (isInvincible != this.isInvincible)
            {
                this.isInvincible = isInvincible;
                if (this.isInvincible)
                    this.currentInvincibleFlash = 0;
            }
            else if (this.isInvincible)
                this.currentInvincibleFlash += Time.deltaTime;
        }

        private Color GetCurrentColor(bool isDashing, bool isDead)
        {
            if (isDead) return deadColour;
            if (isDashing) return dashColour;
            return defaultColour;
        }

        private float GetCurrentAlpha(bool isDead)
        {
            if (!isDead && isInvincible)
            {
                var flashValue = Math.Round(this.currentInvincibleFlash / this.invincibleFlashDuration);
                if (flashValue % 2 == 1)
                    return this.invincibleAlpha;
            }

            return 1f;
        }
    }
}
