using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private PlayerHUD _playerHUD;

    private NetworkObject _boundPlayer;
    private PlayerHUDBinder _hudBinder;
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
        _hudBinder.Bind(_playerHUD);
    }

    private void Unbind()
    {
        if (_hudBinder != null)
            _hudBinder.Unbind();

        _hudBinder = null;
        _boundPlayer = null;
    }
}
