using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private PlayerHUD _playerHUD;
    [SerializeField] private SkillCooldownUI _skillCooldownUI;

    private NetworkObject _boundPlayer;
    private PlayerHUDBinder _hudBinder;
    private SkillCooldownUIBinder _skillCooldownBinder;
    private Coroutine _trackRoutine;

    private void OnEnable()
    {
        _trackRoutine = StartCoroutine(TrackLocalPlayer());
    }

    private void OnDisable()
    {
        if (_trackRoutine != null)
            StopCoroutine(_trackRoutine);

        Unbind();
    }

    private IEnumerator TrackLocalPlayer()
    {
        while (true)
        {
            while (GetLocalPlayer() == null)
                yield return null;

            Bind(GetLocalPlayer());

            while (GetLocalPlayer() == _boundPlayer && _boundPlayer.IsSpawned)
                yield return null;

            Unbind();
        }
    }

    private NetworkObject GetLocalPlayer()
    {
        if (NetworkManager.Singleton == null) return null;
        if (NetworkManager.Singleton.IsClient == false) return null;

        return NetworkManager.Singleton.LocalClient?.PlayerObject;
    }

    private void Bind(NetworkObject player)
    {
        _boundPlayer = player;
        _hudBinder = player.GetComponent<PlayerHUDBinder>();
        _skillCooldownBinder = player.GetComponent<SkillCooldownUIBinder>();
        _hudBinder.Bind(_playerHUD);
        _skillCooldownBinder.Bind(_skillCooldownUI);
    }

    private void Unbind()
    {
        if (_hudBinder != null)
            _hudBinder.Unbind();

        if (_skillCooldownBinder != null)
            _skillCooldownBinder.Unbind();

        _hudBinder = null;
        _skillCooldownBinder = null;
        _boundPlayer = null;
    }
}
