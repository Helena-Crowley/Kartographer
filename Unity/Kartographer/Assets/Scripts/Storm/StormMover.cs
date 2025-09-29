using UnityEngine;

public class StormMover : MonoBehaviour
{
    public float growRate = 1f;       // scale growth per second

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Scale the storm along X in only one direction (positive X)
        float scaleIncrease = growRate * Time.deltaTime;
        transform.localScale += new Vector3(scaleIncrease, 0f, 0f);

        // Offset the position so scaling happens from the left edge
        transform.position += Vector3.right * scaleIncrease * 0.5f;
    }
}
