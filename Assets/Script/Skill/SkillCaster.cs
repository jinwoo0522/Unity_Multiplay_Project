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

        return _stat.Get_Stat(Stat.STAT_TAG.MP) >= data.fManaCost;
    }

    public bool TryCast(NetworkObjectType type, Vector3 position, Vector3 direction)
    {
        if (CanCast(type) == false) return false;

        SkillData data = _skillData[type];

        GameManager.Instance.skillFactory.Create(
            type, position, direction, OwnerClientId, gameObject);

        _stat.Add_Stat(Stat.STAT_TAG.MP, -data.fManaCost);
        _readyTimes[type] = NetworkManager.ServerTime.Time + data.fCooldown;

        return true;
    }

    public float GetRemainingCooldown(NetworkObjectType type)
    {
        if (_readyTimes.TryGetValue(type, out double readyTime) == false) return 0f;

        return Mathf.Max(0f, (float)(readyTime - NetworkManager.ServerTime.Time));
    }
}
