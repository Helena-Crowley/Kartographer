using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject canvasToGoBackTo;
    private GameObject canvasToDeactivate;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoBack()
    {
        canvasToDeactivate.SetActive(false);
        canvasToGoBackTo.SetActive(true);
    }

    public void ResumeGame(GameObject currentCanvas)
    {
        currentCanvas.SetActive(false);
    }
}
