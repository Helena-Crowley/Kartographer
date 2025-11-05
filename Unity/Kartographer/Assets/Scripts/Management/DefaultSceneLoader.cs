using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultSceneLoader : NetworkBehaviour
{
    [Header("Scene to Load")]
    public string sceneToLoad;

    public override void OnNetworkSpawn()
    {
        Debug.Log("DefaultSceneLoader OnNetworkSpawn called");

        if (!IsServer)
        {
            Debug.Log("Not server, scene will not load automatically");
            return;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneToLoad is empty! Please assign a scene name.");
            return;
        }

        Debug.Log("Server loading scene: " + sceneToLoad);
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
    }
}
