using Unity.Netcode;
using UnityEngine;

public class CombatTextPresenter : NetworkBehaviour
{
    private const float HealReportInterval = 0.5f;
    private const float TextHeight = 2f;

    private float _fPendingHeal;
    private float _fHealReportTime;
    private CombatTextUI _combatTextUI;

    public override void OnNetworkSpawn()
    {
        _fPendingHeal = 0f;
        _fHealReportTime = 0f;
    }

    public void Bind(CombatTextUI combatTextUI)
    {
        _combatTextUI = combatTextUI;
    }

    public void Unbind()
    {
        _combatTextUI = null;
    }

    public void Show(CombatTextUI.TextType type, float fAmount, Transform target)
    {
        if (IsServer == false || IsSpawned == false) return;

        if (type == CombatTextUI.TextType.HEAL)
        {
            _fPendingHeal += fAmount;
            return;
        }

        Send(type, fAmount, target.position + Vector3.up * TextHeight);
    }

    private void Update()
    {
        if (IsServer == false || _fPendingHeal <= 0f) return;

        _fHealReportTime += Time.deltaTime;
        if (_fHealReportTime < HealReportInterval) return;

        Send(CombatTextUI.TextType.HEAL, _fPendingHeal,
            transform.position + Vector3.up * TextHeight);
        _fPendingHeal = 0f;
        _fHealReportTime = 0f;
    }

    private void Send(CombatTextUI.TextType type, float fAmount, Vector3 vWorldPosition)
    {
        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } }
        };
        ShowClientRpc(type, fAmount, vWorldPosition, rpcParams);
    }

    [ClientRpc]
    private void ShowClientRpc(CombatTextUI.TextType type, float fAmount,
        Vector3 vWorldPosition, ClientRpcParams rpcParams = default)
    {
        _combatTextUI?.Show(type, fAmount, vWorldPosition);
    }
}
