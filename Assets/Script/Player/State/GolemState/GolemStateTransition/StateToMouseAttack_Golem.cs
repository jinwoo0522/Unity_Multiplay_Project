using UnityEngine;

public class StateToMouseAttack_Golem : ITransition
{
    public ushort NextState => (ushort)GOLEM.StateType.MOUSE_SKILL;

    IEntityInputState _inputState;
    private StateMachine _upperStateMachine;
    private SkillCaster _skillCaster;


    public StateToMouseAttack_Golem(Golem_Player player)
    {
        _inputState = player._input;
        _upperStateMachine = player._upperStateMachine;
        _skillCaster = player._skillCaster;
    }
    public bool CheckRule(float fTimeDelta)
    {
        // 상체가 공격·피격 중이면 전신 스킬로 넘어가지 않는다
        if(_upperStateMachine.CurrentState != (ushort)ENTITY.UpperStateType.IDLE)
            return false;

        if((_inputState.inputState & (ushort)ENTITY.InputFlagType.MOUSE_RIGHT) == 0)
            return false;

        return _skillCaster.CanCast(NetworkObjectType.FIRE_EXPLOSION);
    }

    public void OnTransition()
    {
    }
}
