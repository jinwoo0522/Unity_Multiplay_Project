using UnityEngine;

// 상체 IDLE → 마법사 Q스킬(QSKILL) : Q 입력 시 전환
public class IdleToMagicianQSkill_Magician : ITransition
{
    public ushort NextState => (ushort)MAGICIAN.UpperStateType.QSKILL;

    IEntityInputState _inputState;
    SkillCaster _skillCaster;

    public IdleToMagicianQSkill_Magician(Magician_Player player)
    {
        _inputState = player._input;
        _skillCaster = player._skillCaster;
    }

    public bool CheckRule(float fTimeDelta)
    {
        if((_inputState.inputState & (ushort)ENTITY.InputFlagType.Q) == 0)
            return false;

        return _skillCaster.CanCast(NetworkObjectType.ICE_EXPLOSION);
    }

    public void OnTransition() { }
}
