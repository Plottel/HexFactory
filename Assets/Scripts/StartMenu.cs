using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : UIMenu
{
    private Button _startButton;

    private void Start()
    {
        _startButton = Find<Button>("StartButton");
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        GameManager.Instance.SetPause(false);
        gameObject.SetActive(false);
    }
}
