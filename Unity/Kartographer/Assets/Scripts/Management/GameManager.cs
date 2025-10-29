using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject loadingScreenPrefab;
    private LoadingScreen currentLoadingScreen;

    public int randomSeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadNetworkScene(sceneName));
    }

    private IEnumerator LoadNetworkScene(string sceneName)
    {
        if (loadingScreenPrefab != null)
        {
            GameObject screen = Instantiate(loadingScreenPrefab);
            currentLoadingScreen = screen.GetComponent<LoadingScreen>();
        }

        bool sceneLoaded = false;

        // Listen for Netcode scene load completion
        void OnSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted && sceneEvent.SceneName == sceneName)
            {
                sceneLoaded = true;
                if (currentLoadingScreen != null)
                    Destroy(currentLoadingScreen.gameObject);
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
        }

        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;

        // Start loading the scene across all clients
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        // Optional: animate progress bar
        while (!sceneLoaded)
        {
            if (currentLoadingScreen != null)
                currentLoadingScreen.SetProgress(Mathf.PingPong(Time.time * 0.5f, 1f));
            yield return null;
        }
    }

}
