using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectView : MonoBehaviour
{
    [SerializeField] private Button _magicianButton;
    [SerializeField] private Button _golemButton;
    [SerializeField] private Button _elfButton;
    [SerializeField] private GameObject _hudUI;
    [SerializeField] private GameObject _skillCoolTimeUI;

    public event Action<PLAYER_INDEX> Selected;

    private void OnEnable()
    {
        _magicianButton.onClick.AddListener(SelectMagician);
        _golemButton.onClick.AddListener(SelectGolem);
        _elfButton.onClick.AddListener(SelectElf);
    }

    private void OnDisable()
    {
        _magicianButton.onClick.RemoveListener(SelectMagician);
        _golemButton.onClick.RemoveListener(SelectGolem);
        _elfButton.onClick.RemoveListener(SelectElf);
    }

    private void SelectMagician()
    {
        Selected?.Invoke(PLAYER_INDEX.MAGICIAN);
    }

    private void SelectGolem()
    {
        Selected?.Invoke(PLAYER_INDEX.GOLEM);
    }

    private void SelectElf()
    {
        Selected?.Invoke(PLAYER_INDEX.ELF);
    }

    public void ShowGameUI()
    {
        gameObject.SetActive(false);
        _hudUI.SetActive(true);
        _skillCoolTimeUI.SetActive(true);
    }
}
