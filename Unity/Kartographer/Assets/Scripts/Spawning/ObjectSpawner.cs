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
        // Example: Randomly find a location and raycast down
        //entire building spawnpoint (no corner ref)
        Debug.Log($"Random Seed: {GameManager.Instance.randomSeed}");
        int objectMissedCount = 0;
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
                objectMissedCount++;
                continue;
            }
        }
        if (objectMissedCount > 0)
        {
            float percentMissed = objectMissedCount / (float)count * 100f;
            Debug.LogWarning($"Missed {percentMissed:F1}% of spawn attempts for {objectToSpawn.name}");
        }
    }

    void Start()
    {

        if (GameManager.Instance.randomSeed == 0)
        {
            GameManager.Instance.randomSeed = System.DateTime.Now.Millisecond;
        }
        Random.InitState(GameManager.Instance.randomSeed);
        for (int i = 0; i < objects.Length; i++)
        {
            SpawnObject(objects[i], amounts[i]);
        }

    }


}
