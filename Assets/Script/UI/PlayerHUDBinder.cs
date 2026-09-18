using Unity.Netcode;
using UnityEngine;

public class PlayerHUDBinder : NetworkBehaviour
{
    private Stat _stat;
    private PlayerHUD _hud;

    private void Awake()
    {
        _stat = GetComponent<Stat>();
    }

    public void Bind(PlayerHUD hud)
    {
        Unbind();

        _hud = hud;
        _stat.StatChanged += OnStatChanged;

        _hud.SetName($"client : {OwnerClientId}");
        RefreshHp();
        RefreshMana();
    }

    public void Unbind()
    {
        if (_hud == null) return;

        _stat.StatChanged -= OnStatChanged;
        _hud = null;
    }

    public override void OnNetworkDespawn()
    {
        Unbind();
    }

    private void OnStatChanged(Stat.STAT_TAG tag)
    {
        if (tag == Stat.STAT_TAG.HP || tag == Stat.STAT_TAG.MAX_HP)
            RefreshHp();

        if (tag == Stat.STAT_TAG.MP || tag == Stat.STAT_TAG.MAX_MP)
            RefreshMana();
    }

    private void RefreshHp()
    {
        _hud.SetHp(
            _stat.Get_Stat(Stat.STAT_TAG.HP),
            _stat.Get_Stat(Stat.STAT_TAG.MAX_HP));
    }

    private void RefreshMana()
    {
        _hud.SetMana(
            _stat.Get_Stat(Stat.STAT_TAG.MP),
            _stat.Get_Stat(Stat.STAT_TAG.MAX_MP));
    }
}
