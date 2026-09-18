using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 화면 좌측 하단 HUD — 로컬 플레이어의 이름·HP·마나를 표시하는 씬 단일 오브젝트.
// BattleUIManager가 로컬 플레이어의 PlayerHUDBinder에 전달해 연결한다.
public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Slider          _hpSlider;
    [SerializeField] private Slider          _manaSlider;


    public void SetName(string name) => _nameText.text = name;

    public void SetHp(float cur, float max)
    {
        _hpSlider.maxValue = max;
        _hpSlider.value    = cur;

    }

    public void SetMana(float cur, float max)
    {
        _manaSlider.maxValue = max;
        _manaSlider.value    = cur;
    }
}
