using UnityEngine;

public class PlayerSelectPresenter : MonoBehaviour
{
    [SerializeField] private PlayerSelectView _view;
    [SerializeField] private PlayerSpawner _playerSpawner;

    private void OnEnable()
    {
        _view.Selected += OnPlayerSelected;
    }

    private void OnDisable()
    {
        _view.Selected -= OnPlayerSelected;
    }

    private void OnPlayerSelected(PLAYER_INDEX characterIndex)
    {
        if (_playerSpawner.RequestSpawnPlayer(characterIndex))
            _view.ShowGameUI();
    }
}
