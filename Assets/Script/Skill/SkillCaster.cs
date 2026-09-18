using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillCaster : NetworkBehaviour
{
    public event Action<NetworkObjectType, SkillData> LocalCooldownStarted;

    [System.Serializable]
    private struct SkillEntry
    {
        public NetworkObjectType type;
        public SkillData data;
    }

    [SerializeField] private List<SkillEntry> _skills = new List<SkillEntry>();

    private readonly Dictionary<NetworkObjectType, SkillData> _skillData = new();
    private SkillCooldownModel _serverCooldown;
    private SkillCooldownModel _localCooldown;
    private Stat _stat;

    private void Awake()
    {
        _stat = GetComponent<Stat>();

        for (int i = 0; i < _skills.Count; ++i)
            _skillData.Add(_skills[i].type, _skills[i].data);

        _serverCooldown = new SkillCooldownModel(
            _skillData, () => NetworkManager.ServerTime.Time);
        _localCooldown = new SkillCooldownModel(
            _skillData, () => Time.unscaledTimeAsDouble);
        _localCooldown.CooldownStarted += OnLocalCooldownStarted;
    }

    public bool CanCast(NetworkObjectType type)
    {
        if (IsServer == false) return false;
        if (_skillData.TryGetValue(type, out SkillData data) == false) return false;
        if (_serverCooldown.IsReady(type) == false) return false;

        float fCurrentMana = _stat.Get_Stat(Stat.STAT_TAG.MP);
        return fCurrentMana >= data.fManaCost;
    }

    public bool HasMana()
    {
        if (IsServer == false) return false;

        return _stat.Get_Stat(Stat.STAT_TAG.MP) > 0f;
    }

    public bool ConsumeManaPerSecond(float fManaCostPerSecond, float fTimeDelta)
    {
        if (IsServer == false) return false;

        float fCurrentMana = _stat.Get_Stat(Stat.STAT_TAG.MP);
        float fManaCost = fManaCostPerSecond * fTimeDelta;
        _stat.Set_Stat(Stat.STAT_TAG.MP, Mathf.Max(fCurrentMana - fManaCost, 0f));

        return fCurrentMana <= fManaCost;
    }

    public Skill TryCast(NetworkObjectType type, Vector3 position, Vector3 direction)
    {
        if (CanCast(type) == false) return null;

        SkillData data = _skillData[type];

        Skill skill = GameManager.Instance.skillFactory.Create(
            type, position, direction, OwnerClientId, gameObject);

        _stat.Add_Stat(Stat.STAT_TAG.MP, -data.fManaCost);
        if (data.CooldownTiming == CooldownTiming.ON_CAST)
            StartCooldown(type);

        return skill;
    }

    public float GetRemainingCooldown(NetworkObjectType type)
    {
        return _serverCooldown.GetRemainingCooldown(type);
    }

    public void StartCooldown(NetworkObjectType type)
    {
        if (IsServer == false) return;
        if (_skillData.TryGetValue(type, out SkillData data) == false) return;

        _serverCooldown.StartCooldown(type);
        StartLocalCooldownClientRpc(type, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { OwnerClientId },
            },
        });
    }

    public float GetRemainingLocalCooldown(NetworkObjectType type)
    {
        return _localCooldown.GetRemainingCooldown(type);
    }

    public bool TryGetSkillData(NetworkObjectType type, out SkillData data)
    {
        return _skillData.TryGetValue(type, out data);
    }

    [ClientRpc]
    private void StartLocalCooldownClientRpc(NetworkObjectType type, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner == false) return;

        _localCooldown.StartCooldown(type);
    }

    private void OnLocalCooldownStarted(NetworkObjectType type, SkillData data)
    {
        LocalCooldownStarted?.Invoke(type, data);
    }

}
