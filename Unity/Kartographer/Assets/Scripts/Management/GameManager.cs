using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int randomSeed;
    public float stormDistance;
    [SerializeField] private Transform stormTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    public float StormDistance(Transform player)
    {
        if (player == null) return 0f;

        return player.transform.position.x - stormTransform.position.x;
    }
}