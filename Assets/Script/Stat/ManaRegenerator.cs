// 초당 마나 회복량을 시간에 비례해 적용하는 순수 C# 객체
public class ManaRegenerator
{
    private readonly Stat _stat;
    private readonly float _manaPerSecond;

    public ManaRegenerator(Stat stat, float manaPerSecond)
    {
        _stat = stat;
        _manaPerSecond = manaPerSecond;
    }

    public void Update(float deltaTime)
    {
        float currentMana = _stat.Get_Stat(Stat.STAT_TAG.MP);
        float maxMana = _stat.Get_Stat(Stat.STAT_TAG.MAX_MP);

        if(currentMana >= maxMana) return;

        _stat.RecoverMana(_manaPerSecond * deltaTime, false);
    }
}
