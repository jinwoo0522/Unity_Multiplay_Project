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

        float fHealAmount = _skill.Data.fHealAmountPerSecond * fTimeDelta;
        _stat.Heal(fHealAmount);
    }
}
