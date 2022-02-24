using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapObject : DialogueObject
{
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private float fadeTime;

    public override IEnumerator Hide()
    {
        var alpha = tilemaps[0].color.a;
        do
        {
            alpha = Mathf.Max(0, alpha - Time.deltaTime / this.fadeTime);

            foreach (var tilemap in tilemaps)
            {
                var newColour = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha);
                tilemap.color = newColour;
            }

            yield return null;
        } while (alpha > 0);
    }

    public override IEnumerator Show()
    {
        var alpha = tilemaps[0].color.a;
        do
        {
            alpha = Mathf.Min(1, alpha + Time.deltaTime / this.fadeTime);

            foreach (var tilemap in tilemaps)
            {
                var newColour = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha);
                tilemap.color = newColour;
            }

            yield return null;
        } while (alpha < 1);
    }
}
