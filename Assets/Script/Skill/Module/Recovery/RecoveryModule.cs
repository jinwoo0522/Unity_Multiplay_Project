using UnityEngine;

[System.Serializable]
public class RecoveryModule : ISkillModule
{
    private Skill _skill;
    private Stat _stat;

    public void Bind(Skill skill)
    {
        _skill = skill;
    }

    public void Enter()
    {
        _stat = _skill.Owner.GetComponent<Stat>();
    }

    public void ServerTick(float fTimeDelta)
    {
        _skill.transform.position = _skill.Owner.transform.position;

        float fCurrentHp = _stat.Get_Stat(Stat.STAT_TAG.HP);
        float fMaxHp = _stat.Get_Stat(Stat.STAT_TAG.MAX_HP);
        float fHealAmount = _skill.Data.fHealAmountPerSecond * fTimeDelta;
        _stat.Set_Stat(Stat.STAT_TAG.HP, Mathf.Min(fCurrentHp + fHealAmount, fMaxHp));
    }
}
