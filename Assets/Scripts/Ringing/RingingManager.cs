using Assets.Scripts.Helpers;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private UnityEvent onRing;

    [Header("Activation")]
    [SerializeField] private bool ringWithoutPrompt;
    [SerializeField] private float ringEvery = 5f;

    private Transform currentRingTransform;
    private bool ringing, endOfLevel;
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
        if (this.endOfLevel)
            return;

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

    public static void EndOfLevel(Transform crystal)
    {
        instance.endOfLevel = true;
        instance.currentRingTransform = crystal;

        Ring();
    }

    private IEnumerator RingAnimation()
    {
        if (this.ringing)
            yield break;
        this.ringing = true;

        var (startPosition, endPosition) = this.GetTextPositions();
        if (startPosition == Vector2.zero && endPosition == Vector2.zero)
        {
            this.ringing = false;
            yield break;
        }

        this.ringPointOnBounds = startPosition;
        this.ringingText.transform.localPosition = startPosition;

        this.onRing?.Invoke();

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

    private (Vector2 startPosition, Vector2 endPosition) GetTextPositions()
    {
        Vector2 startPosition;
        Vector2 endPosition;

        if (this.endOfLevel)
        {
            startPosition = this.currentRingTransform.position + (Vector2.up * this.border).ToVector3();
            endPosition = startPosition + Vector2.up * moveDistance;
        }
        else
        {
            var bounds = RingHelpers.GetCameraSize(this.mainCamera);
            var (withinBounds, ringPoint) = RingHelpers.GetPointWithinBounds(bounds, this.currentRingTransform.position, this.border);
            if (withinBounds)
            {
                this.ringing = false;
                return (Vector2.zero, Vector2.zero);
            }

            startPosition = new Vector2(ringPoint.x / canvas.localScale.x, ringPoint.y / canvas.localScale.y);

            var moveDirection = (bounds.center - startPosition).normalized * this.moveDistance;
            endPosition = startPosition + moveDirection;
        }

        return (startPosition, endPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.ringPointOnBounds, 0.5f);
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
