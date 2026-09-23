using Unity.Netcode;
using UnityEngine;
using System;

// 엔티티 수치 저장소 — 값의 보관·동기화·변경 통지만 담당한다
// 피격 판정과 사망 처리는 EntityDamage가 이 통지를 구독해 처리한다
public class Stat : NetworkBehaviour
{
    public enum STAT_TAG
    {
        HP,
        MAX_HP,
        MP,
        MAX_MP,
        DAMAGE,
        WALK_SPEED,
        RUN_SPEED,
        END,
    }
    // 네트워크 동기화가 필요 없는 원본 설정값을 외부가 읽도록 노출 (쓰기는 막는다)
    public Entity_Data _data => Stat_Data;

    [SerializeField] private Entity_Data Stat_Data;
    private NetworkList<float> StatList = new();
    private ManaRegenerator _manaRegenerator;

    // 스탯 변경 통지 — UI·피격 처리 등 외부가 구독 (Stat은 구독자를 모름, 단방향)
    public event Action<STAT_TAG> StatChanged;

    public override void OnNetworkSpawn()
    {
        if(IsServer == true)
        {
            for(int i = StatList.Count ; i < (int)STAT_TAG.END ; i++)
                StatList.Add(0f);

            StatList[(int)STAT_TAG.HP] = Stat_Data.fMaxHp;
            StatList[(int)STAT_TAG.MAX_HP] = Stat_Data.fMaxHp;
            StatList[(int)STAT_TAG.MP] = Stat_Data.fMaxMana;
            StatList[(int)STAT_TAG.MAX_MP] = Stat_Data.fMaxMana;
            StatList[(int)STAT_TAG.DAMAGE] = Stat_Data.fAttackDamage;
            StatList[(int)STAT_TAG.WALK_SPEED] = Stat_Data.fWalkSpeed;
            StatList[(int)STAT_TAG.RUN_SPEED] = Stat_Data.fRunSpeed;

            _manaRegenerator = new ManaRegenerator(this, Stat_Data.fManaRegen);
        }

        // 초기값 설정이 끝난 뒤 변경 이벤트를 구독한다.
        StatList.OnListChanged += HandleListChanged;
    }

    private void Update()
    {
        if(IsServer == false) return;

        _manaRegenerator?.Update(Time.deltaTime);
    }

    public override void OnNetworkDespawn()
    {
        StatList.OnListChanged -= HandleListChanged;
    }

    // NetworkList 변경을 STAT_TAG 단위 이벤트로 변환해 외부에 전달
    private void HandleListChanged(NetworkListEvent<float> e)
    {
        StatChanged?.Invoke((STAT_TAG)e.Index);
    }

    public float Get_Stat(STAT_TAG tag)
    {
        return StatList[(int)tag];
    }

    public float TakeDamage(float fDamage, Transform attacker)
    {
        if (IsServer == false || fDamage <= 0f) return 0f;

        float fAppliedDamage = Mathf.Min(Get_Stat(STAT_TAG.HP), fDamage);
        Set_Stat(STAT_TAG.HP, Get_Stat(STAT_TAG.HP) - fAppliedDamage);

        if (fAppliedDamage > 0f && attacker != null && attacker.gameObject != gameObject
            && attacker.TryGetComponent(out CombatTextPresenter presenter))
            presenter.Show(CombatTextUI.TextType.DAMAGE, fAppliedDamage, transform);

        return fAppliedDamage;
    }

    public float Heal(float fAmount)
    {
        if (IsServer == false || fAmount <= 0f) return 0f;

        float fCurrentHp = Get_Stat(STAT_TAG.HP);
        float fAppliedHeal = Mathf.Min(fAmount, Get_Stat(STAT_TAG.MAX_HP) - fCurrentHp);
        if (fAppliedHeal <= 0f) return 0f;

        Set_Stat(STAT_TAG.HP, fCurrentHp + fAppliedHeal);
        if (TryGetComponent(out CombatTextPresenter presenter))
            presenter.Show(CombatTextUI.TextType.HEAL, fAppliedHeal, transform);

        return fAppliedHeal;
    }

    public float RecoverMana(float fAmount, bool isTextVisible = true)
    {
        if (IsServer == false || fAmount <= 0f) return 0f;

        float fCurrentMana = Get_Stat(STAT_TAG.MP);
        float fRecoveredMana = Mathf.Min(fAmount, Get_Stat(STAT_TAG.MAX_MP) - fCurrentMana);
        if (fRecoveredMana <= 0f) return 0f;

        Set_Stat(STAT_TAG.MP, fCurrentMana + fRecoveredMana);
        if (isTextVisible && TryGetComponent(out CombatTextPresenter presenter))
            presenter.Show(CombatTextUI.TextType.MANA, fRecoveredMana, transform);

        return fRecoveredMana;
    }
    public void Set_Stat(STAT_TAG tag , float fValue)
    {
        if(IsServer == false) return;
        
        StatList[(int)tag] = fValue;
    }
    public void Add_Stat(STAT_TAG tag , float fValue)
    {
        if(IsServer == false) return;

        StatList[(int)tag] += fValue;
    }
}
