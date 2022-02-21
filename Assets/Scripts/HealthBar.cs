using Assets.Scripts.Models.Player;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider primarySlider;
    [SerializeField] private Slider secondarySlider;

    [SerializeField] private float transitionDuration;
    [SerializeField] private AnimationCurve transitionCurve;

    private IEnumerator transition;

    public void ResetHealth(PlayerHealth health)
    {
        primarySlider.maxValue = health.MaxHealth;
        primarySlider.value = health.CurrentHealth;

        secondarySlider.maxValue = health.MaxHealth;
        secondarySlider.value = health.CurrentHealth;
    }

    public void UpdateHealth(PlayerHealth health)
    {
        primarySlider.value = health.CurrentHealth;

        if (this.transition != null)
            StopCoroutine(this.transition);

        this.transition = this.UpdateSecondaryHealthBar(health.CurrentHealth);
        StartCoroutine(this.transition);
    }

    private IEnumerator UpdateSecondaryHealthBar(float newHealth)
    {
        var startingHealth = secondarySlider.value;
        var difference = startingHealth - newHealth;
        var currentTransitionTime = 0f;

        while (this.secondarySlider.value > newHealth)
        {
            this.secondarySlider.value = startingHealth - (difference * this.transitionCurve.Evaluate(currentTransitionTime / this.transitionDuration));

            yield return new WaitForEndOfFrame();
            currentTransitionTime += Time.deltaTime;
        }

        this.transition = null;
    }
}
