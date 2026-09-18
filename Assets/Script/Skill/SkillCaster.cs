using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillCaster : NetworkBehaviour
{
    [System.Serializable]
    private struct SkillEntry
    {
        public NetworkObjectType type;
        public SkillData data;
    }

    [SerializeField] private List<SkillEntry> _skills = new List<SkillEntry>();

    private readonly Dictionary<NetworkObjectType, SkillData> _skillData = new();
    private readonly Dictionary<NetworkObjectType, double> _readyTimes = new();
    private Stat _stat;

    private void Awake()
    {
        _stat = GetComponent<Stat>();

        for (int i = 0; i < _skills.Count; ++i)
            _skillData.Add(_skills[i].type, _skills[i].data);
    }

    public bool CanCast(NetworkObjectType type)
    {
        if (IsServer == false) return false;
        if (_skillData.TryGetValue(type, out SkillData data) == false) return false;
        if (GetRemainingCooldown(type) > 0f) return false;

        float fCurrentMana = _stat.Get_Stat(Stat.STAT_TAG.MP);
        return fCurrentMana >= data.fManaCost;
    }

    public bool HasMana()
    {
        if (IsServer == false) return false;

        return _stat.Get_Stat(Stat.STAT_TAG.MP) > 0f;
    }

    public void ConsumeManaPerSecond(float fManaCostPerSecond, float fTimeDelta)
    {
        if (IsServer == false) return;

        float fCurrentMana = _stat.Get_Stat(Stat.STAT_TAG.MP);
        float fManaCost = fManaCostPerSecond * fTimeDelta;
        _stat.Set_Stat(Stat.STAT_TAG.MP, Mathf.Max(fCurrentMana - fManaCost, 0f));
    }

    public Skill TryCast(NetworkObjectType type, Vector3 position, Vector3 direction)
    {
        if (CanCast(type) == false) return null;

        SkillData data = _skillData[type];

        Skill skill = GameManager.Instance.skillFactory.Create(
            type, position, direction, OwnerClientId, gameObject);

        _stat.Add_Stat(Stat.STAT_TAG.MP, -data.fManaCost);
        _readyTimes[type] = NetworkManager.ServerTime.Time + data.fCooldown;

        return skill;
    }

    public float GetRemainingCooldown(NetworkObjectType type)
    {
        if (_readyTimes.TryGetValue(type, out double readyTime) == false) return 0f;

        return Mathf.Max(0f, (float)(readyTime - NetworkManager.ServerTime.Time));
    }
}
