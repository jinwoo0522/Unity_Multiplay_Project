using DG.Tweening;
using TMPro;
using UnityEngine;

public class CombatTextItem : MonoBehaviour, IPoolable
{
    private const float DamagePopScale = 5f;
    private const float DamagePopDuration = 0.08f;
    private const float DamageReturnDuration = 0.25f;

    public IPoolReturner Handler { get; set; }

    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private CanvasGroup _canvasGroup;

    private RectTransform _rectTransform;
    private Sequence _sequence;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
    }

    public void Active()
    {
        gameObject.SetActive(true);
    }

    public void Release()
    {
        gameObject.SetActive(false);
        transform.SetParent(GameManager.Instance.transform, false);
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    public void Show(string message, Color color, Vector2 vPosition, bool isDOScale = false)
    {
        _sequence?.Kill();
        _label.text = message;
        _label.color = color;
        _canvasGroup.alpha = 1f;
        _rectTransform.localScale = Vector3.one;
        _rectTransform.anchoredPosition = vPosition;

        _sequence = DOTween.Sequence()
            .Join(_rectTransform.DOAnchorPosY(vPosition.y + 80f, 1f).SetEase(Ease.OutQuad))
            .Insert(0.4f, _canvasGroup.DOFade(0f, 0.6f));

        if (isDOScale == true)
            _sequence.Insert(0f, _rectTransform.DOScale(DamagePopScale, DamagePopDuration).SetEase(Ease.OutQuad))
                .Insert(DamagePopDuration, _rectTransform.DOScale(1f, DamageReturnDuration).SetEase(Ease.OutQuad));

        _sequence.OnComplete(() =>
        {
            _sequence = null;
            Handler.Return(this);
        });
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}
