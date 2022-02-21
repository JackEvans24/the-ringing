using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class CollisionHelper
    {
        [SerializeField] private Transform[] topChecks;
        [SerializeField] private Transform[] bottomChecks;
        [SerializeField] private Transform[] leftChecks;
        [SerializeField] private Transform[] rightChecks;
        [SerializeField] private float collisionDistance = 0.05f;

        public bool Hit(int layerMask, out List<Collider2D> colliders)
        {
            var topResult = this.HitTop(layerMask, out var topColliders);
            var leftResult = this.HitLeft(layerMask, out var leftColliders);
            var rightResult = this.HitRight(layerMask, out var rightColliders);
            var bottomResult = this.HitBottom(layerMask, out var bottomColliders);

            colliders = topColliders.Union(leftColliders).Union(rightColliders).Union(bottomColliders).ToList();

            return topResult || leftResult || rightResult || bottomResult;
        }

        public bool HitTop(int layerMask, out List<Collider2D> collisions) => this.Hit(this.topChecks, Vector2.up, layerMask, out collisions);
        public bool HitBottom(int layerMask, out List<Collider2D> collisions) => this.Hit(this.bottomChecks, Vector2.down, layerMask, out collisions);
        public bool HitLeft(int layerMask, out List<Collider2D> collisions) => this.Hit(this.leftChecks, Vector2.left, layerMask, out collisions);
        public bool HitRight(int layerMask, out List<Collider2D> collisions) => this.Hit(this.rightChecks, Vector2.right, layerMask, out collisions);

        private bool Hit(Transform[] checks, Vector2 direction, int layerMask, out List<Collider2D> collisions)
        {
            collisions = new List<Collider2D>();

            foreach (var point in checks)
            {
                var hits = Physics2D.RaycastAll(point.position, direction, this.collisionDistance, layerMask);
                if (hits.Any())
                    collisions.AddRange(hits.Select(hit => hit.collider));
            }

            return collisions.Any();
        }

        public void DrawGizmos()
        {
            // Draw top checks
            foreach (var check in topChecks)
                Gizmos.DrawLine(check.position, check.position + Vector3.up * this.collisionDistance);
            // Draw bottom checks
            foreach (var check in bottomChecks)
                Gizmos.DrawLine(check.position, check.position + Vector3.down * this.collisionDistance);
            // Draw left checks
            foreach (var check in leftChecks)
                Gizmos.DrawLine(check.position, check.position + Vector3.left * this.collisionDistance);
            // Draw right checks
            foreach (var check in rightChecks)
                Gizmos.DrawLine(check.position, check.position + Vector3.right * this.collisionDistance);

        }
    }
}
