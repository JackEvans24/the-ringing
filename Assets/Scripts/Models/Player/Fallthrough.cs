using Assets.Scripts.Constants;
using System;
using UnityEngine;

namespace Assets.Scripts.Models.Player
{
    [Serializable]
    public class Fallthrough
    {
        [SerializeField] private float layerSwitchTime;
        private float currentSwitchTime;

        public bool ShouldFall { get => this.currentSwitchTime < this.layerSwitchTime; }

        public void Update(GameObject player, bool fallthrough)
        {
            if (currentSwitchTime < this.layerSwitchTime)
                currentSwitchTime += Time.deltaTime;
            else
            {
                var layer = fallthrough ? Layers.PLAYER_FALLTHROUGH : Layers.PLAYER;
                if (player.layer != (int)layer)
                {
                    currentSwitchTime = 0f;
                    player.layer = (int)layer;
                }
            }
        }

    }
}
