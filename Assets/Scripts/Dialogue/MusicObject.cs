using System.Collections;
using UnityEngine;

public class MusicObject : DialogueObject
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField, Range(0f, 1f)] private float audioLevel;
    [SerializeField] private float fadeTime;

    public override IEnumerator Hide()
    {
        var startVolume = this.audioSource.volume;
        var currentTimer = 0f;

        float volume;
        do
        {
            currentTimer += Time.deltaTime;
            volume = Mathf.Max(0f, startVolume * (1 - (currentTimer / this.fadeTime)));
            this.audioSource.volume = volume;

            yield return null;
        } while (volume > 0);
    }

    public override IEnumerator Show()
    {
        var startVolume = this.audioSource.volume;
        var fullVolume = this.audioLevel;
        var currentTimer = 0f;

        float volume;
        do
        {
            currentTimer += Time.deltaTime;
            volume = Mathf.Min(1f, startVolume + (fullVolume * (currentTimer / this.fadeTime)));
            this.audioSource.volume = volume;

            yield return null;
        } while (volume < 1);
    }
}
