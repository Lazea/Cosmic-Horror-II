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
    public GameObject menuPanel;
    public GameObject optionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        if(gameplayPanel != null)
            gameplayPanel.SetActive(true);
        
        if(interactionPromptText != null)
            interactionPromptText.gameObject.SetActive(false);

        if(pausePanel != null)
            pausePanel.SetActive(false);

        if(menuPanel != null)
            menuPanel.SetActive(true);

        if(optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void ShowInteractionPrompt(string msg)
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = msg;
            interactionPromptText.gameObject.SetActive(true);
        }
    }

    public void HideInteractionPrompt()
    {
        if(interactionPromptText != null)
            interactionPromptText.gameObject.SetActive(false);
    }

    public void ShowPausePanel()
    {
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ShowGameplayPanel()
    {
        pausePanel.SetActive(false);
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }
}
