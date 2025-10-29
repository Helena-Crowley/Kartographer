using Unity.Netcode;
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
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var player = client.PlayerObject;
                if (player != null)
                {
                    // Example spawn position
                    player.transform.position = new Vector3(Random.Range(0, .5f), 0, Random.Range(0, .5f));
                }
            }
        }
    }

}
