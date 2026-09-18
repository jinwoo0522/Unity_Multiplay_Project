using System;

public class QSkillManaToIdle_Elf : ITransition
{
    public ushort NextState => (ushort)ENTITY.StateType.IDLE;

    private Func<bool> _isManaDepleted;

    public QSkillManaToIdle_Elf(Func<bool> isManaDepleted)
    {
        _isManaDepleted = isManaDepleted;
    }

    public bool CheckRule(float fTimeDelta)
    {
        return _isManaDepleted();
    }

    public void OnTransition()
    {
    }
}
