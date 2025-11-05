using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkSceneManager : NetworkBehaviour
{
    [Header("UI")]
    public GameObject loadingScreen;
    public Image loadingBarFill;

    [Header("Scene & Spawn Data")]
    [SerializeField] private SpawnData spawnPoints;

    [Header("Input")]
    [SerializeField] private InputActionReference toggleScene;

    [Header("Debug")]
    [SerializeField] private bool debugOn = true;

    private bool playerInTrigger = false;
    private GameObject currentPlayer;
    private Scene previousScene;
    private string sceneToTransitionTo;


    // Keep track of the currently active loaded scene
    private Scene loadedScene;

    private void Start()
    {
        loadingScreen.SetActive(false);
        loadingBarFill.fillAmount = 0f;

        sceneToTransitionTo = spawnPoints.sceneName;

        // Initially, assume the first active scene is loaded
        loadedScene = SceneManager.GetActiveScene();
    }

    private void OnEnable()
    {
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    private void Update()
    {
        if (playerInTrigger && toggleScene.action.WasPressedThisFrame())
        {
            Log($"Player pressed input, requesting scene load: {sceneToTransitionTo}");
            LoadScene(sceneToTransitionTo);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = true;
        currentPlayer = other.gameObject;

        var prompt = currentPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt("E", "Go to " + sceneToTransitionTo);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;

        var prompt = currentPlayer.GetComponentInChildren<InteractPrompt>();
        if (prompt != null)
            prompt.ToggleInteractPrompt();

        currentPlayer = null;
    }

    public void LoadScene(string sceneName)
    {
        if (!IsServer) return;

        if (string.IsNullOrEmpty(sceneName))
        {
            LogWarning("Scene name is empty. Cannot load scene.");
            return;
        }

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Log($"Scene {sceneName} is already loaded.");
            return;
        }

        Log($"Switching scenes: {loadedScene.name} -> {sceneName}");
        StartCoroutine(SwitchSceneRoutine(sceneName));
    }

    private IEnumerator SwitchSceneRoutine(string sceneName)
    {
        loadingScreen.SetActive(true);
        loadingBarFill.fillAmount = 0f;

        // Load the new scene additively
        var loadStatus = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        if (loadStatus != SceneEventProgressStatus.Started)
        {
            LogWarning($"Failed to start loading scene {sceneName}, status: {loadStatus}");
            loadingScreen.SetActive(false);
            yield break;
        }

        // Wait until the new scene is fully loaded
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        while (!newScene.isLoaded)
        {
            loadingBarFill.fillAmount = Mathf.MoveTowards(loadingBarFill.fillAmount, 0.9f, Time.deltaTime * 0.5f);
            yield return null;
        }

        loadedScene = newScene;

        // Determine the spawn data for this scene
        SpawnData sceneSpawnData = spawnPoints;
        if (spawnPoints != null && spawnPoints.sceneName != sceneName)
        {
            LogWarning($"SpawnData scene '{spawnPoints.sceneName}' does not match loaded scene '{sceneName}'. Using default positions.");
            sceneSpawnData = null;
        }

        // Move all players to the new scene and set spawn positions
        if (NetworkManager.Singleton != null)
        {
            int spawnIndex = 0;
            foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
            {
                var player = clientPair.Value.PlayerObject;
                if (player != null)
                {
                    // Move player to the scene
                    SceneManager.MoveGameObjectToScene(player.gameObject, loadedScene);

                    // Apply spawn position and rotation if available
                    if (sceneSpawnData != null && sceneSpawnData.positions.Length > 0)
                    {
                        Vector3 pos = sceneSpawnData.positions[spawnIndex % sceneSpawnData.positions.Length];
                        Quaternion rot = sceneSpawnData.rotations.Length > spawnIndex
                            ? sceneSpawnData.rotations[spawnIndex]
                            : Quaternion.identity;

                        player.transform.SetPositionAndRotation(pos, rot);

                        if (player.TryGetComponent<NetworkTransform>(out var netTransform))
                            netTransform.Teleport(pos, rot, Vector3.one);

                        spawnIndex++;
                    }
                }
            }
        }

        // Complete the loading bar
        loadingBarFill.fillAmount = 1f;
        yield return null;

        // Unload the previous additive scene (skip BootStrap)
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != "BootStrap" && scene != loadedScene)
            {
                var unloadStatus = NetworkManager.SceneManager.UnloadScene(scene);
                if (unloadStatus != SceneEventProgressStatus.Started)
                    LogWarning($"Failed to unload scene {scene.name}, status: {unloadStatus}");
                else
                    Log($"Unloading old additive scene {scene.name}");
            }
        }

        loadingScreen.SetActive(false);
        loadingBarFill.fillAmount = 0f;
        Log($"Scene switched to {sceneName}");
    }





    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        // Only handle server events for debugging
        if (!IsServer) return;

        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                Log($"Scene {sceneEvent.SceneName} loaded on server.");
                break;
            case SceneEventType.UnloadComplete:
                Log($"Scene {sceneEvent.SceneName} unloaded on server.");
                break;
            case SceneEventType.LoadEventCompleted:
                Log($"All clients finished loading scene {sceneEvent.SceneName}.");
                break;
            case SceneEventType.UnloadEventCompleted:
                Log($"All clients finished unloading scene {sceneEvent.SceneName}.");
                break;
        }
    }

    private void Log(string message)
    {
        if (debugOn) Debug.Log("[NetworkSceneManager] " + message);
    }

    private void LogWarning(string message)
    {
        if (debugOn) Debug.LogWarning("[NetworkSceneManager] " + message);
    }
}
