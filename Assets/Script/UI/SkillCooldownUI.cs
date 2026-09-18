using UnityEngine;
using UnityEngine.UI;

// 스킬 쿨타임 HUD 컨트롤러 (owner 로컬 전용, NetworkBehaviour 아님).
// 오버레이 Image의 fillAmount로 쿨타임 진행을 표시한다.
// 슬롯 0=Q(Buff), 슬롯 1=우클릭(Attack_Skill).
public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private Image[] _overlays;

    private readonly float[] _remainingTimes = { -1f, -1f };
    private readonly float[] _totalTimes = new float[2];

    public void SetCooldown(int iSlot, float fRemainingTime, float fTotalTime)
    {
        if (IsValidSlot(iSlot) == false) return;

        _totalTimes[iSlot] = fTotalTime;
        _remainingTimes[iSlot] = fRemainingTime;

        RefreshOverlay(iSlot);
    }

    private void Update()
    {
        for (int i = 0; i < _remainingTimes.Length; ++i)
        {
            if (_remainingTimes[i] <= 0f) continue;

            _remainingTimes[i] = Mathf.Max(0f, _remainingTimes[i] - Time.unscaledDeltaTime);
            RefreshOverlay(i);
        }
    }

    private bool IsValidSlot(int iSlot)
    {
        return iSlot >= 0 && iSlot < _overlays.Length;
    }

    private void RefreshOverlay(int iSlot)
    {
        bool isCoolingDown = _remainingTimes[iSlot] > 0f && _totalTimes[iSlot] > 0f;
        _overlays[iSlot].gameObject.SetActive(isCoolingDown);
        _overlays[iSlot].fillAmount = isCoolingDown
            ? _remainingTimes[iSlot] / _totalTimes[iSlot]
            : 0f;
    }

}
