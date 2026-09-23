using System.Collections;
using Unity.Netcode;
using UnityEngine;

// 플레이어의 머리 위 UI와 사망 완료 신호를 담당한다.
public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Canvas canvas;

    private SpectatorCamera _spectatorCamera;

    public override void OnNetworkSpawn()
    {
        WorldHPPresenter worldHpPresenter = canvas.GetComponent<WorldHPPresenter>();
        worldHpPresenter.SetName($"client : {OwnerClientId}");

        if(IsOwner == false) return;

        _spectatorCamera = GameManager.Instance.cameraManager
            .Get_Cinemachine(CameraManager.CinemachineTag.PLAYER)
            .GetComponent<SpectatorCamera>();

        _spectatorCamera.BindPlayer(transform, canvas);
        canvas.gameObject.SetActive(false);
    }

    public void BeginSpectatingAfterDissolve()
    {
        StartCoroutine(SpectateAfterDissolve());
    }

    private IEnumerator SpectateAfterDissolve()
    {
        yield return new WaitForSeconds(GetComponent<Stat>()._data.fDissolveDuration);
        FinishDeath_ClientRpc();
    }

    [ClientRpc]
    private void FinishDeath_ClientRpc()
    {
        if(IsOwner == true)
            _spectatorCamera.StartSpectating();
        gameObject.SetActive(false);
    }
}
