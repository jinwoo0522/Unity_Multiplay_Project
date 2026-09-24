using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SessionManager
{
    public const int MaxPlayers = 5;
    public const int MinimumMatchPlayers = 2;
    public const string GameSceneName = "Dungeon_Level_1";
    public const string IntroSceneName = "Intro";

    public static SessionManager Instance { get; } = new SessionManager();
    
    public ISession CurrentSession { get; private set; }
    public bool IsAutomaticMatch { get; private set; }

    public async Task<ISession> StartMatchmakingAsync()
    {
        IsAutomaticMatch = true;
        _isGameStarting = false;
        await SignInAsync();

        var quickJoinOptions = new QuickJoinOptions
        {
            CreateSession = true,
            Filters = new List<FilterOption>
            {
                new FilterOption(FilterField.AvailableSlots, "1", FilterOperation.GreaterOrEqual),
                new FilterOption(FilterField.StringIndex1, PublicMatch, FilterOperation.Equal)
            }
        };

        CurrentSession = await MultiplayerService.Instance.MatchmakeSessionAsync(
            quickJoinOptions, CreateSessionOptions(false));
        WatchConnection();

        if (CurrentSession.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            await Task.Yield();
            TryStartGame();
        }

        return CurrentSession;
    }

    public async Task<ISession> CreateRoomAsync()
    {
        IsAutomaticMatch = false;
        await SignInAsync();
        CurrentSession = await MultiplayerService.Instance.CreateSessionAsync(CreateSessionOptions(true));
        WatchConnection();
        return CurrentSession;
    }

    public async Task<ISession> JoinRoomAsync(string joinCode)
    {
        IsAutomaticMatch = false;
        await SignInAsync();
        CurrentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(
            joinCode.Trim().ToUpperInvariant(), new JoinSessionOptions { Type = SessionType });
        WatchConnection();
        return CurrentSession;
    }

    private const string SessionType = "GolemVsMagician";
    private const string MatchTypeProperty = "matchType";
    private const string PublicMatch = "public";
    private bool _isGameStarting;
    private bool _isReturningToIntro;

    private SessionManager()
    {
    }

    private void WatchConnection()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        networkManager.OnTransportFailure += OnTransportFailure;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        MultiplayerService.Instance.SessionRemoved += OnSessionRemoved;
    }

    private void OnTransportFailure()
    {
        BeginReturnToIntro();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
            BeginReturnToIntro();
    }

    private void OnSessionRemoved(ISession session)
    {
        if (session == CurrentSession)
            BeginReturnToIntro();
    }

    private void BeginReturnToIntro()
    {
        if (_isReturningToIntro) return;

        _isReturningToIntro = true;
        _ = ReturnToIntroAsync();
    }

    private async Task ReturnToIntroAsync()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        ISession session = CurrentSession;

        try
        {
            if (networkManager != null)
            {
                networkManager.OnTransportFailure -= OnTransportFailure;
                networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
                networkManager.OnClientConnectedCallback -= OnClientConnected;
            }
            MultiplayerService.Instance.SessionRemoved -= OnSessionRemoved;

            await Task.Yield();
            if (networkManager != null && networkManager.IsListening && !networkManager.ShutdownInProgress)
                networkManager.Shutdown();

            while (networkManager != null && (networkManager.IsListening || networkManager.ShutdownInProgress))
                await Task.Yield();

            if (session != null && session.State == SessionState.Connected)
            {
                if (session.IsHost)
                    await session.AsHost().DeleteAsync();
                else
                    await session.LeaveAsync();
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"세션 정리 중 오류가 발생했습니다: {exception.Message}");
        }

        CurrentSession = null;
        IsAutomaticMatch = false;
        _isGameStarting = false;

        GameManager gameManager = UnityEngine.Object.FindAnyObjectByType<GameManager>();
        if (gameManager != null)
            UnityEngine.Object.Destroy(gameManager.gameObject);
        if (networkManager != null)
            UnityEngine.Object.Destroy(networkManager.gameObject);

        await Task.Yield();
        SceneManager.LoadScene(IntroSceneName);
        _isReturningToIntro = false;
    }

    private async void OnClientConnected(ulong clientId)
    {
        // 연결 콜백이 끝나야 진행 중인 씬 동기화 이벤트도 종료된다.
        await Task.Yield();
        TryStartGame();
    }

    private void TryStartGame()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (_isReturningToIntro || !IsAutomaticMatch || !CurrentSession.IsHost || _isGameStarting ||
            networkManager.ConnectedClientsList.Count < MinimumMatchPlayers)
            return;

        SceneEventProgressStatus status = networkManager.SceneManager.LoadScene(
            GameSceneName, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogError($"자동 매칭 게임 씬을 열지 못했습니다: {status}");
            return;
        }

        _isGameStarting = true;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    private static SessionOptions CreateSessionOptions(bool isPrivate)
    {
        var options = new SessionOptions
        {
            MaxPlayers = MaxPlayers,
            Type = SessionType,
            IsPrivate = isPrivate
        };

        if (!isPrivate)
        {
            options.SessionProperties = new Dictionary<string, SessionProperty>
            {
                { MatchTypeProperty, new SessionProperty(PublicMatch, VisibilityPropertyOptions.Public, PropertyIndex.String1) }
            };
        }

        return options.WithRelayNetwork();
    }

    private static async Task SignInAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
