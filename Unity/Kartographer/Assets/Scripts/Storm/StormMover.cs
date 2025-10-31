using UnityEngine;
using Unity.Netcode;

public class StormMover : NetworkBehaviour
{
    public float growRate = 1f;       // scale growth per second

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (!IsServer) return;
        // Scale the storm along X in only one direction (positive X)
        float scaleIncrease = growRate * Time.deltaTime;
        transform.localScale += new Vector3(scaleIncrease, 0f, 0f);

        // Offset the position so scaling happens from the left edge
        transform.position += Vector3.right * scaleIncrease * 0.5f;

        UpdateStormPositionClientRpc(transform.position, transform.localScale);
    }

    [ClientRpc]
    void UpdateStormPositionClientRpc(Vector3 newPosition, Vector3 newScale)
    {
        if (IsServer) return; // server already updated

        transform.position = newPosition;
        transform.localScale = newScale;
    }
}
