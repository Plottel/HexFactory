using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : UIMenu
{
    public Button PauseButton;

    private Button _startButton;

    private void Start()
    {
        _startButton = Find<Button>("StartButton");
        _startButton.onClick.AddListener(OnStartButtonClicked);

        PauseButton.gameObject.SetActive(false);

        if (GameManager.SkipStartMenuOnSceneLoad)
            OnStartButtonClicked();
    }

    private void OnStartButtonClicked()
    {
        GameManager.Instance.SetPause(false);
        gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(true);
    }
}
