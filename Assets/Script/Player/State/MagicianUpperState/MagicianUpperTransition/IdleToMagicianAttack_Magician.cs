using UnityEngine;

// 상체 IDLE → 마법사 공격(ATTACK) : 마우스 우클릭 입력 시 전환
public class IdleToMagicianAttack_Magician : ITransition
{
    public ushort NextState => (ushort)MAGICIAN.UpperStateType.ATTACK;

    IEntityInputState _inputState;
    SkillCaster _skillCaster;

    public IdleToMagicianAttack_Magician(Magician_Player player)
    {
        _inputState = player._input;
        _skillCaster = player._skillCaster;
    }

    public bool CheckRule(float fTimeDelta)
    {
        if((_inputState.inputState & (ushort)ENTITY.InputFlagType.MOUSE_LEFT) == 0)
            return false;

        return _skillCaster.CanCast(NetworkObjectType.ELECTRONIC_SKILL);
    }

    public void OnTransition() { }
}
