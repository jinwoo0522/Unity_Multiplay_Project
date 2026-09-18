using UnityEngine;

public enum CooldownTiming
{
    ON_CAST,
    ON_END,
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public float fSpeed;
    public float fLifetime;
    public float fDamage;
    public float fRadius;
    public float fKnockbackPower;
    public float fKnockbackDecay;
    public float fAirbornePower;
    public float fAirborneDecay;
    public float fFreezeTime;
    public float fManaCost;
    public float fManaCostPerSecond;
    public float fHealAmountPerSecond;
    public float fCooldown;
    [SerializeField] private CooldownTiming _cooldownTiming;

    public CooldownTiming CooldownTiming => _cooldownTiming;
}
