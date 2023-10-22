using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Gameplay Panel")]
    public GameObject gameplayPanel;
    public TextMeshProUGUI interactionPromptText;

    [Header("Menu Panel")]
    public GameObject pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        gameplayPanel.SetActive(true);
        interactionPromptText.gameObject.SetActive(false);

        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowInteractionPrompt(string msg)
    {
        interactionPromptText.text = msg;
        interactionPromptText.gameObject.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        interactionPromptText.gameObject.SetActive(false);
    }

    public void ShowPausePanel()
    {
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ShowGameplayPanel()
    {
        pausePanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }
}
