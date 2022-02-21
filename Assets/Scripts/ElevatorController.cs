using Assets.Scripts.Constants;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ElevatorController : MonoBehaviour
{
    private PlayerInput input;

    [SerializeField] private CanvasGroup readyPrompt;
    [SerializeField] private float readyPromptAppearTime;

    [Header("Elevator path")]
    [SerializeField] private bool atBeginning;
    [SerializeField] private Vector2 elevatorPath;
    [SerializeField] private float travelTime;
    [SerializeField] private Ease travelEase;

    [Header("Player shift")]
    [SerializeField] private float playerShiftTime;
    [SerializeField] private Ease playerShiftEase;

    private GameObject player;
    private bool isActive;
    private Tween fade;

    private void Awake()
    {
        this.input = GetComponent<PlayerInput>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.PLAYER))
        {
            this.player = collision.gameObject;
            this.SetPromptFade(1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.PLAYER))
        {
            this.player = null;
            this.SetPromptFade(0f);
        }
    }

    private void SetPromptFade(float fadeLevel)
    {
        if (this.fade != null && this.fade.active)
            this.fade.Kill();

        if (this.readyPrompt != null)
            this.fade = this.readyPrompt.DOFade(fadeLevel, this.readyPromptAppearTime);
    }

    private void Update()
    {
        if (this.player == null || this.isActive)
            return;

        if (this.input.actions[PlayerInputButton.ACTION].triggered)
            StartLift();
    }

    private void StartLift()
    {
        this.isActive = true;
        this.SetPromptFade(0f);

        this.player.transform.parent = this.transform;
        var playerController = this.player.GetComponent<PlayerController>();
        playerController.CanMove = false;

        var coefficient = this.atBeginning ? 1 : -1;
        var targetPosition = new Vector3(
            this.transform.position.x + (elevatorPath.x * coefficient),
            this.transform.position.y + (elevatorPath.y * coefficient),
            this.transform.position.z
        );

        var sequence = DOTween.Sequence();

        sequence.Append(this.player.transform.DOMoveX(this.transform.position.x, this.playerShiftTime)).SetEase(this.playerShiftEase);
        sequence.Append(this.transform.DOMove(targetPosition, this.travelTime).SetEase(this.travelEase));

        sequence.OnComplete(() =>
        {
            this.atBeginning = !this.atBeginning;
            
            this.player.transform.parent = null;
            playerController.CanMove = true;

            this.isActive = false;
            this.SetPromptFade(1f);
        });
    }
}
