using UnityEngine;

public class ElfMouseSkillState : EntityState
{
    Elf_Player _player;
    EntityAnimator _aniController;
    IEntityMovement _move;
    IEntityRotate _rotate;
    private StateMachine _upperStateMachine;

    const float fDashSpeed = 10f;
    const float fDashDistance = 3f;

    public ElfMouseSkillState(Elf_Player player)
    {
        _player = player;
        _aniController = player._aniController;
        _move = player._move;
        _rotate = player._rotate;
        _upperStateMachine = player._upperStateMachine;
    }
    public override void Create()
    {
        TransitionList.Add(new StateToIdle_Player(_aniController));
        StateEvents.Add((0.25f , EventFunc));
        StateEvents.Add((0.75f , CastSkill));
    }

    public override void Enter()
    {
        _aniController._state.Value = (ushort)ELF.StateType.MOUSE_SKILL;
        _aniController._animator.applyRootMotion = true;
        _upperStateMachine.TransitionTo((ushort)ENTITY.UpperStateType.EMPTY);   // 상체 잠금
    }

    public override void Exit()
    {
        _aniController._animator.applyRootMotion = false;
        _upperStateMachine.TransitionTo((ushort)ENTITY.UpperStateType.IDLE);    // 상체 복귀
    }

    protected override void UpdateState(float fTimedelta, ushort curState)
    {
        _move.Gravity();
        _rotate.Rotate();
    }
    void EventFunc()
    {
        _move.Dash(fDashSpeed , fDashDistance);
    }

    void CastSkill()
    {
        _player._skillCaster.TryCast(
            NetworkObjectType.ELF_MOUSE_SKILL, _player.transform.position, _player.transform.forward);
    }
}
