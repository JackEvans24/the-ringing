using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class RingingManager : MonoBehaviour
{
    private static RingingManager instance;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private TMP_Text ringingText;
    [SerializeField] private Transform awakeRingingPosition;

    [Header("Effect")]
    [SerializeField] private float border = 1f;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private float fadePause = 0.5f;
    [SerializeField] private Ease fadeEasing = Ease.InOutSine;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveTime = 2f;
    [SerializeField] private Ease moveEasing = Ease.InOutSine;

    [Header("Activation")]
    [SerializeField] private bool ringWithoutPrompt;
    [SerializeField] private float ringEvery = 5f;

    private Transform currentRingTransform;
    private bool ringing;
    private float currentRingTimer;
    private Vector2 ringPointOnBounds;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        instance = this;

        if (this.awakeRingingPosition != null)
            SetRingingPosition(this.awakeRingingPosition);
    }

    private void Update()
    {
        if (!this.ringWithoutPrompt)
        {
            this.currentRingTimer = 0f;
            return;
        }

        this.currentRingTimer += Time.deltaTime;
        if (this.currentRingTimer >= this.ringEvery)
        {
            Ring();
            this.currentRingTimer = 0f;
        }
    }

    public static void SetRingingPosition(Transform ringTransform)
    {
        instance.currentRingTransform = ringTransform;
    }

    public static void Ring()
    {
        instance.StartCoroutine(instance.RingAnimation());
    }

    private IEnumerator RingAnimation()
    {
        if (this.ringing)
            yield break;
        this.ringing = true;

        var bounds = RingHelpers.GetCameraSize(this.mainCamera);
        var (withinBounds, ringPoint) = RingHelpers.GetPointWithinBounds(bounds, this.currentRingTransform.position, this.border);
        if (withinBounds)
        {
            this.ringing = false;
            yield break;
        }

        var startPosition = new Vector2(ringPoint.x / canvas.localScale.x, ringPoint.y / canvas.localScale.y);

        var moveDirection =  (bounds.center - startPosition).normalized * this.moveDistance;
        var endPosition = startPosition + moveDirection;

        this.ringPointOnBounds = startPosition;
        this.ringingText.transform.localPosition = startPosition;

        var moveTween = this.ringingText.transform
            .DOLocalMove(endPosition, this.moveTime)
            .SetEase(this.moveEasing);

        yield return this.ringingText
            .DOFade(1f, this.fadeTime)
            .SetEase(this.fadeEasing)
            .WaitForCompletion();

        yield return new WaitForSeconds(this.fadePause);

        yield return this.ringingText
            .DOFade(0f, this.fadeTime)
            .SetEase(this.fadeEasing)
            .WaitForCompletion();

        yield return moveTween.WaitForCompletion();

        this.ringing = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.ringPointOnBounds, 0.5f);
    }
}
