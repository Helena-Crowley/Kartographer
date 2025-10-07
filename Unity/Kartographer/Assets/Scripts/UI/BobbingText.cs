using UnityEngine;

public class BobbingText : MonoBehaviour
{
    [SerializeField] private float bobSpeed = 2f;   // How fast it bobs
    [SerializeField] private float bobHeight = 0.1f; // How high it moves
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
