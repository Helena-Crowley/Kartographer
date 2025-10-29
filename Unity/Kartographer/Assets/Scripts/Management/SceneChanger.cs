using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChanger : NetworkBehaviour
{
    [SerializeField] private string outpostScene = "OutPost";
    [SerializeField] private string desertScene = "DefaultScene";

    [SerializeField] private InputActionReference toggleAction;

    private bool toggle = false;

    void Update()
    {
        if (!IsServer) return; // Only the server can change scenes

        // Example: press 'N' to go to the next scene
        if (toggleAction.action.WasPressedThisFrame())
        {
            if (!toggle) NetworkManager.SceneManager.LoadScene(outpostScene, LoadSceneMode.Single);
            else NetworkManager.SceneManager.LoadScene(desertScene, LoadSceneMode.Single);

            toggle = !toggle;
        }
    }

    private void Start()
    {
        if (IsServer)
            NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            if (!IsServer) return;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var player = client.PlayerObject;
                if (player != null)
                {
                    // Move player to spawn inside building
                    player.GetComponent<NetworkTransform>().Teleport(new Vector3(1f, 0.5f, 1f), Quaternion.identity, Vector3.one);
                }
            }
        }
    }


}
