using UnityEngine;
using System.Collections;

public class DestructibleCactus : MonoBehaviour
{
    public GameObject[] pieces;
    public AudioClip breakSound;

    private bool broken = false;

    void Start()
    {
        foreach (GameObject rend in pieces)
        {
            foreach (Renderer childRend in rend.GetComponentsInChildren<Renderer>())
            {
                childRend.sharedMaterial = childRend.sharedMaterial;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (broken) return;

        if (other.CompareTag("Cart"))
        {
            Rigidbody cartRb = other.attachedRigidbody;
            if (cartRb == null || cartRb.linearVelocity.magnitude < 10f) return;


            Break(other.ClosestPoint(transform.position));
        }
    }

    void Break(Vector3 impactPoint)
    {
        broken = true;

        foreach (var piece in pieces)
        {
            piece.SetActive(true);

            // Only add Rigidbody when breaking
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb == null)
                rb = piece.AddComponent<Rigidbody>();

            rb.mass = 0.1f; // keep it light
            rb.isKinematic = false;
            rb.AddExplosionForce(50f, impactPoint, 1.5f);

            if (breakSound)
                StartCoroutine(PlayBreakSoundWithDelay(piece.transform.position));
        }

        // Disable main mesh and collider
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;

        var collider = GetComponent<Collider>();
        if (collider) collider.enabled = false;

        // Optional cleanup after a few seconds
        Destroy(gameObject, 4f);
    }

    private IEnumerator PlayBreakSoundWithDelay(Vector3 pos)
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.25f));
        SoundManager.Instance.PlaySound(breakSound, pos, 0.3f, true);
    }
}
