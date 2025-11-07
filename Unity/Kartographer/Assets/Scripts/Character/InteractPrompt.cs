using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    public GameObject interactPrompt;
    public TMP_Text textComponent;

    private void Start()
    {
        PlayerUIManager.Instance.BindPlayer(this);
        interactPrompt.SetActive(false);
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
                textComponent.text = "Press '" + keyToPress + "' to " + action;
        }
        else
        {
            interactPrompt.SetActive(false);
            textComponent.text = "No key or action assigned.";
        }
    }
}


