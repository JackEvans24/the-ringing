using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Models.Player
{
    public class PlayerInputConverter
    {
        public Inputs CurrentInputs;

        private PlayerInput input;

        private bool startJump;
        private bool sustainJump;

        private bool fallthrough;

        public void PerformSetup(PlayerInput input)
        {
            this.input = input;

            this.input.actions[PlayerInputButton.JUMP].started += Jump_Started;
            this.input.actions[PlayerInputButton.JUMP].canceled += Jump_Cancelled;

            this.input.actions[PlayerInputButton.FALLTHROUGH].started += (_) => this.fallthrough = true;
            this.input.actions[PlayerInputButton.FALLTHROUGH].canceled += (_) => this.fallthrough = false;
        }

        private void Jump_Started(InputAction.CallbackContext obj)
        {
            this.startJump = true;
            this.sustainJump = true;
        }

        private void Jump_Cancelled(InputAction.CallbackContext obj)
        {
            this.sustainJump = false;
        }

        public void UpdateInputs()
        {
            var movement = this.input.actions[PlayerInputButton.MOVEMENT].ReadValue<Vector2>();
            var dash = this.input.actions[PlayerInputButton.DASH].triggered;
            var action = this.input.actions[PlayerInputButton.ACTION].triggered;

            var result = new Inputs()
            {
                Movement = movement.x,
                StartJump = this.startJump,
                SustainJump = this.sustainJump,
                Dash = dash,
                Fallthrough = this.fallthrough,
                Action = action
            };

            this.startJump = false;

            this.CurrentInputs = result;
        }

        public struct Inputs
        {
            public float Movement { get; set; }
            public bool StartJump { get; set; }
            public bool SustainJump { get; set; }
            public bool Dash { get; set; }
            public bool Fallthrough { get; set; }
            public bool Action { get; set; }
        }
    }
}
