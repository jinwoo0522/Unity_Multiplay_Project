using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroView : MonoBehaviour
{
    public event Action MatchmakingRequested;
    public event Action RoomCreationRequested;
    public event Action RoomJoinRequested;

    public string JoinCode => _joinCodeInput.text;

    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _joinRoomButton;
    [SerializeField] private Button _startMatchButton;
    [SerializeField] private TMP_InputField _joinCodeInput;
    [SerializeField] private TMP_Text _sessionMessage;
    [SerializeField] private CanvasGroup _roomUserScroll;

    public void SetConnecting(bool isConnecting, string message)
    {
        _createRoomButton.interactable = !isConnecting;
        _joinRoomButton.interactable = !isConnecting;
        _startMatchButton.interactable = !isConnecting;
        _sessionMessage.text = message;
    }

    public void ShowJoinedSession(string sessionCode, bool isCreatedRoom)
    {
        _sessionMessage.text = isCreatedRoom ? $"방 코드: {sessionCode}" : "방에 입장했습니다.";
        _roomUserScroll.alpha = isCreatedRoom ? 1f : 0f;
        _roomUserScroll.interactable = isCreatedRoom;
        _roomUserScroll.blocksRaycasts = isCreatedRoom;
        _createRoomButton.gameObject.SetActive(false);
        _joinRoomButton.gameObject.SetActive(false);
        _startMatchButton.gameObject.SetActive(false);
        _joinCodeInput.gameObject.SetActive(false);
    }

    public void ShowInputRequired()
    {
        _sessionMessage.text = "방 코드를 입력하세요.";
    }

    private void OnEnable()
    {
        _createRoomButton.onClick.AddListener(OnRoomCreationRequested);
        _joinRoomButton.onClick.AddListener(OnRoomJoinRequested);
        _startMatchButton.onClick.AddListener(OnMatchmakingRequested);
    }

    private void OnDisable()
    {
        _createRoomButton.onClick.RemoveListener(OnRoomCreationRequested);
        _joinRoomButton.onClick.RemoveListener(OnRoomJoinRequested);
        _startMatchButton.onClick.RemoveListener(OnMatchmakingRequested);
    }

    private void OnMatchmakingRequested()
    {
        MatchmakingRequested?.Invoke();
    }

    private void OnRoomCreationRequested()
    {
        RoomCreationRequested?.Invoke();
    }

    private void OnRoomJoinRequested()
    {
        RoomJoinRequested?.Invoke();
    }
}
