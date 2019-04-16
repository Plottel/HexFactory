using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : UIMenu
{
    public Button PauseButton;

    private Button _restartButton;
    private Button _quitButton;

    private void Awake()
    {
        _restartButton = Find<Button>("RestartButton");
        _quitButton = Find<Button>("QuitButton");

        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);

        GameManager.Instance.eventGameOver += Show;

        gameObject.SetActive(false);
    }

    void OnRestartButtonClicked()
    {
        GameManager.Instance.ResetGame(true);
    }

    void OnQuitButtonClicked()
    {
        GameManager.Instance.ResetGame(false);
    }

    void Show()
    {
        PauseButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GameManager.Instance.SetPause(true);
    }
}
