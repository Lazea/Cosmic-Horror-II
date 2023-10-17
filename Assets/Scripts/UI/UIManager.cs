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
    public GameObject menuPanel;

    // Start is called before the first frame update
    void Start()
    {
        gameplayPanel.SetActive(true);
        interactionPromptText.gameObject.SetActive(false);

        menuPanel.SetActive(false);
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
}
