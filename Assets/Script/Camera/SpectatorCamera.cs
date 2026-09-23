using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// 로컬 카메라에서 플레이어 추적과 사망 후 관전 입력을 관리한다.
public class SpectatorCamera : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;

    private CinemachineCamera _camera;
    private Camera _mainCamera;
    private InputActionAsset _spectateActions;
    private InputAction _nextAction;
    private InputAction _previousAction;
    private bool _isSpectating;
    private ulong _targetClientId;

    public void BindPlayer(Transform player, Canvas canvas)
    {
        StopSpectating();
        _mainCamera = GameManager.Instance.cameraManager.Get_Camera(CameraManager.CameraTag.MAIN);
        canvas.worldCamera = _mainCamera;
        _targetClientId = player.GetComponent<NetworkObject>().OwnerClientId;
        _camera.Target.TrackingTarget = player;
    }

    public void StartSpectating()
    {
        _isSpectating = true;
        _nextAction.performed += OnNextPerformed;
        _previousAction.performed += OnPreviousPerformed;
        _nextAction.Enable();
        _previousAction.Enable();
        ChangeTarget(true);
    }

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
        _spectateActions = Instantiate(_inputActions);
        _nextAction = _spectateActions.FindAction("UI/SpectateNext", true);
        _previousAction = _spectateActions.FindAction("UI/SpectatePrevious", true);
    }

    private void OnDisable()
    {
        StopSpectating();
    }

    private void OnDestroy()
    {
        Destroy(_spectateActions);
    }

    private void StopSpectating()
    {
        if(_isSpectating == false) return;

        _isSpectating = false;
        _nextAction.performed -= OnNextPerformed;
        _previousAction.performed -= OnPreviousPerformed;
        _nextAction.Disable();
        _previousAction.Disable();
    }

    private void OnNextPerformed(InputAction.CallbackContext context)
    {
        ChangeTarget(true);
    }

    private void OnPreviousPerformed(InputAction.CallbackContext context)
    {
        ChangeTarget(false);
    }

    private void ChangeTarget(bool isNext)
    {
        List<NetworkObject> players = new();
        foreach (NetworkObject networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            if(networkObject.IsPlayerObject == false || networkObject.gameObject.activeInHierarchy == false) continue;

            Stat stat = networkObject.GetComponent<Stat>();
            if(stat.Get_Stat(Stat.STAT_TAG.HP) <= 0f) continue;

            players.Add(networkObject);
        }

        players.Sort((left, right) => left.OwnerClientId.CompareTo(right.OwnerClientId));
        if(players.Count == 0) return;

        int iCurrent = players.FindIndex(player => player.OwnerClientId == _targetClientId);
        int iNext = iCurrent < 0
            ? (isNext ? 0 : players.Count - 1)
            : (iCurrent + (isNext ? 1 : players.Count - 1)) % players.Count;

        NetworkObject target = players[iNext];
        _targetClientId = target.OwnerClientId;
        _camera.Target.TrackingTarget = target.transform;
    }
}
