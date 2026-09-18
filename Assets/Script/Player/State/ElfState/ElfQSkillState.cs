using UnityEngine;

public class ElfQSkillState : EntityState
{
    private Elf_Player _player;
    private Skill _skill;
    EntityAnimator _aniController;
    IEntityMovement _move;
    IEntityRotate _rotate;
    IEntityInputState _input;
    private StateMachine _upperStateMachine;
    public ElfQSkillState(Elf_Player player)
    {
        _player = player;
        _aniController = player._aniController;
        _move = player._move;
        _rotate = player._rotate;
        _input = player._input;
        _upperStateMachine = player._upperStateMachine;
    }
    public override void Create()
    {
        TransitionList.Add(new QSkillToIdle_Elf(_input));
        TransitionList.Add(new QSkillManaToIdle_Elf(_player._skillCaster));
    }

    public override void Enter()
    {
        _aniController._state.Value = (ushort)ELF.StateType.Q_SKILL;
        _skill = _player._skillCaster.TryCast(
            NetworkObjectType.ELF_Q_SKILL, _player.transform.position, _player.transform.forward);
        _upperStateMachine.TransitionTo((ushort)ENTITY.UpperStateType.EMPTY);   // 상체 잠금
    }

    public override void Exit()
    {
        if(_skill.NetworkObject.IsSpawned == true)
            _skill.NetworkObject.Despawn();

        _upperStateMachine.TransitionTo((ushort)ENTITY.UpperStateType.IDLE);    // 상체 복귀
    }

    protected override void UpdateState(float fTimedelta, ushort curState)
    {
        _player._skillCaster.ConsumeManaPerSecond(_skill.Data.fManaCostPerSecond, fTimedelta);
        _move.Gravity();
        _rotate.Rotate();
    }

}
