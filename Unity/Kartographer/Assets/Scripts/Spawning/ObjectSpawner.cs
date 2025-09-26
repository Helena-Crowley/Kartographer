using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objects; //Likely an array of building prefabs
    public int[] amounts; //How many of each to spawn
    public LayerMask terrainLayer; // Assign the "Ground" Layer here
    public float yOffset = 0f; //adjust in case we have origin offsets to account for

    public GameObject parentObject; // Parent object to hold spawned objects

    [SerializeField] private InputActionReference spawnAction;

    void SpawnObject(GameObject objectToSpawn, int count) // Spawn buildings at completely random points on terrain
    {
        Debug.Log("Spawning Objects");
        // Example: Randomly find a location and raycast down
        //entire building spawnpoint (no corner ref)

        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(-500, 500);
            float randomZ = Random.Range(-200, 200);

            Vector3 spawnOrigin = new Vector3(randomX, 300f, randomZ);
            // Shoot ray down
            Ray ray = new Ray(spawnOrigin + Vector3.up * 500f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
            {
                //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 2f);
                Quaternion rotation = Quaternion.LookRotation(Vector3.up, Vector3.up);

                GameObject spawnedObject = Instantiate(objectToSpawn, hit.point, rotation);
                spawnedObject.transform.parent = parentObject.transform;
            }
            else
            {
                Debug.Log($"assett {i} did not hit terrain.");
                continue;
            }
        }
    }

    void Update()
    {
        if (spawnAction.action.WasPressedThisFrame())
        {
            Debug.Log("Spawn Action Pressed");
            for (int i = 0; i < objects.Length; i++)
            {
                SpawnObject(objects[i], amounts[i]);
            }
        }
    }


}
