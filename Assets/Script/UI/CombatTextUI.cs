using UnityEngine;

public class CombatTextUI : MonoBehaviour
{
    private const float DamageHorizontalSpread = 40f;

    public enum TextType : byte
    {
        DAMAGE,
        FREEZE,
        AIRBORNE,
        HEAL,
        MANA,
    }

    [SerializeField] private Color _damageColor = Color.white;
    [SerializeField] private Color _freezeColor = new Color(0.35f, 0.8f, 1f);
    [SerializeField] private Color _airborneColor = new Color(0.75f, 0.45f, 1f);
    [SerializeField] private Color _healColor = new Color(0.35f, 1f, 0.45f);
    [SerializeField] private Color _manaColor = new Color(0.25f, 0.55f, 1f);

    private RectTransform _canvasRect;
    private Camera _camera;

    private void Awake()
    {
        _canvasRect = (RectTransform)transform;
    }

    private void OnDisable()
    {
        foreach (CombatTextItem item in GetComponentsInChildren<CombatTextItem>(true))
            item.Handler.Return(item);
    }

    public void Show(TextType type, float fAmount, Vector3 vWorldPosition)
    {
        if (_camera == null)
            _camera = Camera.main;

        if (_camera == null) return;

        Vector3 vScreen = _camera.WorldToScreenPoint(vWorldPosition);
        if (vScreen.z <= 0f) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, vScreen, null, out Vector2 vPosition)) return;

        if (type == TextType.FREEZE || type == TextType.AIRBORNE)
            vPosition.y += 32f;
        else if (type == TextType.MANA)
            vPosition.y -= 32f;

        if (type == TextType.DAMAGE)
            vPosition.x += Random.Range(-DamageHorizontalSpread, DamageHorizontalSpread);

        CombatTextItem item = GameManager.Instance.objectPoolManager.Get<CombatTextItem>(PoolObjectType.COMBAT_TEXT);
        item.transform.SetParent(_canvasRect, false);

        string message = type switch
        {
            TextType.FREEZE => "빙결",
            TextType.AIRBORNE => "에어본",
            TextType.HEAL => $"+{fAmount:0.##}",
            TextType.MANA => $"+{fAmount:0.##}",
            _ => fAmount.ToString("0.##"),
        };

        Color color = type switch
        {
            TextType.FREEZE => _freezeColor,
            TextType.AIRBORNE => _airborneColor,
            TextType.HEAL => _healColor,
            TextType.MANA => _manaColor,
            _ => _damageColor,
        };

        item.transform.SetAsLastSibling();
        item.Show(message, color, vPosition, type == TextType.DAMAGE);
    }
}
