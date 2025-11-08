using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    public GameObject interactPrompt;
    public TMP_Text textComponent;

    private Color originalColor;

    private void Start()
    {
        PlayerUIManager.Instance.BindPlayer(this);
        interactPrompt.SetActive(false);
        originalColor = textComponent.color;
    }

    /// <summary>
    /// Toggles the on-screen interaction prompt.  
    /// Pass a key and action to show it, or leave blank to hide.
    /// </summary>
    /// 
    public void ToggleInteractPrompt(string keyToPress = "", string action = "", bool hold = false)
    {
        if (!string.IsNullOrEmpty(keyToPress) && !string.IsNullOrEmpty(action))
        {
            interactPrompt.SetActive(true);
            if (!hold)
                textComponent.text = "Press '" + keyToPress + "' to " + action;
            else
                textComponent.text = "Hold '" + keyToPress + "' to " + action;
        }
        else
        {
            interactPrompt.SetActive(false);
            textComponent.text = "No key or action assigned.";
        }
    }

    public void CustomPrompt(string message = "", Color? textColor = null)
    {
        Color colorToUse = textColor ?? Color.aliceBlue;
        if (!string.IsNullOrEmpty(message))
        {
            interactPrompt.SetActive(true);
            textComponent.color = colorToUse;
            textComponent.text = message;
        }
        else
        {
            textComponent.color = originalColor;
            interactPrompt.SetActive(false);
            textComponent.text = "No key or action assigned.";
        }
    }

    public void HidePrompt()
    {
        textComponent.color = originalColor; // stored somewhere globally
        interactPrompt.SetActive(false);
        textComponent.text = string.Empty;
    }

}


