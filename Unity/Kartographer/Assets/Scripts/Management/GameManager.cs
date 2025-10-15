using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int randomSeed;
    public float stormDistance;
    [SerializeField] private Transform stormTransform;

    public GameObject[] playersInCart;

    private void Awake()
    {
        playersInCart = new GameObject[2]; //amount of players that can fit in cart
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