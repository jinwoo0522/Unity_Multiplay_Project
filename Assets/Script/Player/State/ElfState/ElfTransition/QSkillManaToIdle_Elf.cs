using UnityEngine;

public class QSkillManaToIdle_Elf : ITransition
{
    public ushort NextState => (ushort)ENTITY.StateType.IDLE;

    private SkillCaster _skillCaster;

    public QSkillManaToIdle_Elf(SkillCaster skillCaster)
    {
        _skillCaster = skillCaster;
    }

    public bool CheckRule(float fTimeDelta)
    {
        return _skillCaster.HasMana() == false;
    }

    public void OnTransition()
    {
    }
}
