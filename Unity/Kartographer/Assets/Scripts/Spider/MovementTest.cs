using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float distance = 5f; // How far to move from the start position
    public float time = 2f;     // Time to go from one side to the other

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time / time, 1f); // Goes 0 → 1 → 0
        transform.position = startPos + Vector3.forward * distance * t;
    }
}
