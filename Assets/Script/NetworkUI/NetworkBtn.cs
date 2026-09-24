using System;
using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;

public class NetworkBtn : MonoBehaviour
{
    [SerializeField] private IntroView _view;

    private SessionManager _sessionManager;
    private bool _isConnecting;

    private void Awake()
    {
        _sessionManager = SessionManager.Instance;
    }

    private void OnEnable()
    {
        _view.MatchmakingRequested += StartMatchmaking;
        _view.RoomCreationRequested += CreateRoom;
        _view.RoomJoinRequested += JoinRoom;
    }

    private void OnDisable()
    {
        _view.MatchmakingRequested -= StartMatchmaking;
        _view.RoomCreationRequested -= CreateRoom;
        _view.RoomJoinRequested -= JoinRoom;
    }

    private void StartMatchmaking()
    {
        _ = ConnectAsync(_sessionManager.StartMatchmakingAsync, "자동 매칭 중...", false);
    }

    private void CreateRoom()
    {
        _ = ConnectAsync(_sessionManager.CreateRoomAsync, "방 생성 중...", true);
    }

    private void JoinRoom()
    {
        string joinCode = _view.JoinCode;
        if (string.IsNullOrWhiteSpace(joinCode))
        {
            _view.ShowInputRequired();
            return;
        }

        _ = ConnectAsync(() => _sessionManager.JoinRoomAsync(joinCode), "방 입장 중...", false);
    }

    private async Task ConnectAsync(Func<Task<ISession>> connect, string message, bool isCreatedRoom)
    {
        if (_isConnecting) return;
        _isConnecting = true;
        _view.SetConnecting(true, message);

        try
        {
            ISession session = await connect();
            if (_view != null)
                _view.ShowJoinedSession(session.Code, isCreatedRoom);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (_view != null)
                _view.SetConnecting(false, "연결에 실패했습니다. 다시 시도해 주세요.");
        }
        finally
        {
            _isConnecting = false;
        }
    }
}
