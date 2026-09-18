using System;
using Unity.Netcode;
using UnityEngine;

public class SkillCooldownUIBinder : NetworkBehaviour
{
    [Serializable]
    private struct SlotBinding
    {
        [SerializeField] private NetworkObjectType _skillType;
        [SerializeField] private int _slot;

        public NetworkObjectType SkillType => _skillType;
        public int Slot => _slot;
    }

    [SerializeField] private SlotBinding[] _slotBindings;

    private SkillCaster _skillCaster;
    private SkillCooldownUI _ui;

    private void Awake()
    {
        _skillCaster = GetComponent<SkillCaster>();
    }

    public void Bind(SkillCooldownUI ui)
    {
        Unbind();

        _ui = ui;
        _skillCaster.LocalCooldownStarted += OnCooldownStarted;

        for (int i = 0; i < _slotBindings.Length; ++i)
        {
            SlotBinding binding = _slotBindings[i];
            if (_skillCaster.TryGetSkillData(binding.SkillType, out SkillData data) == false) continue;

            _ui.SetIcon(binding.Slot, data.Icon);
            _ui.SetCooldown(
                binding.Slot,
                _skillCaster.GetRemainingLocalCooldown(binding.SkillType),
                data.fCooldown);
        }
    }

    public void Unbind()
    {
        if (_ui == null) return;

        _skillCaster.LocalCooldownStarted -= OnCooldownStarted;
        _ui = null;
    }

    public override void OnNetworkDespawn()
    {
        Unbind();
    }

    private void OnCooldownStarted(NetworkObjectType type, SkillData data)
    {
        for (int i = 0; i < _slotBindings.Length; ++i)
        {
            SlotBinding binding = _slotBindings[i];
            if (binding.SkillType != type) continue;

            _ui.SetCooldown(binding.Slot, data.fCooldown, data.fCooldown);
            return;
        }
    }
}
