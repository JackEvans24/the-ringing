using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerObject : SpriteObject
{
    [Header("References")]
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Rigidbody2D rb;

    public override IEnumerator Hide()
    {
        this.rb.bodyType = RigidbodyType2D.Static;
        this.collider2d.enabled = false;

        yield return base.Hide();
    }

    public override IEnumerator Show()
    {
        yield return base.Show();

        this.collider2d.enabled = true;
        this.rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
