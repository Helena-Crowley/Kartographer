using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TMP_Text textComponent;

    /// <summary>
    /// Toggles the on-screen interaction prompt.  
    /// Pass a key and action to show it, or leave blank to hide.
    /// </summary>
    public void ToggleInteractPrompt(string keyToPress = "", string action = "")
    {
        if (!string.IsNullOrEmpty(keyToPress) && !string.IsNullOrEmpty(action))
        {
            interactPrompt.SetActive(true);
            textComponent.text = "Press '" + keyToPress + "' to " + action;
        }
        else
        {
            interactPrompt.SetActive(false);
            textComponent.text = "No key or action assigned.";
        }
    }
}
