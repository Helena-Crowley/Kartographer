using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public int randomSeed;
    public int buildingsFound = 0;
    public int totalBuildings = 0;
    public float worldXWidth;
    public float worldZWidth;
    public Vector3 worldCenter;

    [SerializeField] private Renderer terrainRenderer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        worldXWidth = terrainRenderer.bounds.extents.x;
        worldZWidth = terrainRenderer.bounds.extents.z;
        worldCenter = terrainRenderer.bounds.center;

        DontDestroyOnLoad(gameObject);
    }
}
