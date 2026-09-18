using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillCooldownModel
{
    public event Action<NetworkObjectType, SkillData> CooldownStarted;

    private readonly Dictionary<NetworkObjectType, SkillData> _skillData;
    private readonly Dictionary<NetworkObjectType, double> _readyTimes = new();
    private readonly Func<double> _getTime;

    public SkillCooldownModel(
        Dictionary<NetworkObjectType, SkillData> skillData,
        Func<double> getTime)
    {
        _skillData = skillData;
        _getTime = getTime;
    }

    public bool IsReady(NetworkObjectType type)
    {
        return GetRemainingCooldown(type) <= 0f;
    }

    public float GetRemainingCooldown(NetworkObjectType type)
    {
        if (_readyTimes.TryGetValue(type, out double dReadyTime) == false) return 0f;

        return Mathf.Max(0f, (float)(dReadyTime - _getTime()));
    }

    public void StartCooldown(NetworkObjectType type)
    {
        SkillData data = _skillData[type];
        _readyTimes[type] = _getTime() + data.fCooldown;
        CooldownStarted?.Invoke(type, data);
    }
}
