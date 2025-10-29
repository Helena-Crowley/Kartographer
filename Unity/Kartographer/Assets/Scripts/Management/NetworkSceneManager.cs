using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Netcode.Components;

public class NetworkSceneManager : NetworkBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string outpostScene = "OutPost";
    [SerializeField] private string desertScene = "Desert";

    [Header("Spawn Data")]
    [SerializeField] private SpawnData[] allSpawnData; // Drag your SpawnData assets here

    [Header("Input")]
    [SerializeField] private InputActionReference toggleSceneAction;

    private bool toggle = false;

    private void Start()
    {
        if (IsServer)
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    private void Update()
    {
        if (!IsServer) return;

        if (toggleSceneAction != null && toggleSceneAction.action.WasPressedThisFrame())
        {
            string sceneToLoad = toggle ? desertScene : outpostScene;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
            toggle = !toggle;
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;
        if (!IsServer) return;

        SpawnData data = GetSpawnData(sceneEvent.SceneName);
        if (data == null || data.spawnPositions.Length == 0) return;

        int i = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                Vector3 pos = data.spawnPositions[i % data.spawnPositions.Length];
                Quaternion rot = data.spawnRotations[i % data.spawnRotations.Length];

                var networkTransform = client.PlayerObject.GetComponent<NetworkTransform>();
                if (networkTransform != null)
                    networkTransform.Teleport(pos, rot, Vector3.one);
                else
                    client.PlayerObject.transform.SetPositionAndRotation(pos, rot);

                i++;
            }
        }
    }

    private SpawnData GetSpawnData(string sceneName)
    {
        foreach (var data in allSpawnData)
        {
            if (data.sceneName == sceneName)
                return data;
        }
        return null;
    }
}
