public interface IMovementModule
{
    void Bind(Skill skill);
    void Enter();
    void Move(float fTimeDelta);
}
