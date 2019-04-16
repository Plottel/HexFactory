using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIMenu
{
    public Button PauseButton;

    private Button _resumeButton;
    private Button _restartButton;
    private Button _quitButton;

    private void Awake()
    {
        _resumeButton = Find<Button>("ResumeButton");
        _restartButton = Find<Button>("RestartButton");
        _quitButton = Find<Button>("QuitButton");

        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);

        PauseButton.onClick.AddListener(OnPauseButtonClicked);

        gameObject.SetActive(false);
    }

    void OnResumeButtonClicked()
    {
        PauseButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
        GameManager.Instance.SetPause(false);
    }

    void OnRestartButtonClicked()
    {
        GameManager.Instance.ResetGame(true);
    }

    void OnQuitButtonClicked()
    {
        GameManager.Instance.ResetGame(false);
    }

    void OnPauseButtonClicked()
    {
        PauseButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GameManager.Instance.SetPause(true);
    }
}
