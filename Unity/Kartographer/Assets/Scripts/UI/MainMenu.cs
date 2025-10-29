using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject networkCanvas;

    public void OnPlayButtonPressed()
    {
        mainMenuCanvas.SetActive(false);
        networkCanvas.SetActive(true);
    }

    public void GoBackToMenu()
    {
        networkCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
