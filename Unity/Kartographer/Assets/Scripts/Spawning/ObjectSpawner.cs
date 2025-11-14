using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objects; //what to spawn
    public int[] amounts; //how many to spawn
    public LayerMask terrainLayer;
    public float yOffset = 0f; //adjust in case we have origin offsets to account for

    public GameObject parentObject; // hold spawned objects in hierarchy

    [SerializeField] private GameObject terrain;
    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    //[SerializeField] private InputActionReference spawnAction;

    void SpawnObject(GameObject objectToSpawn, int count)
    {
        int objectMissedCount = 0;
        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnOrigin = new Vector3(randomX, 300f, randomZ);

            Ray ray = new Ray(spawnOrigin + Vector3.up * 500f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
            {
                Quaternion rotation = Quaternion.LookRotation(Vector3.up, Vector3.up);

                GameObject spawnedObject = Instantiate(objectToSpawn, hit.point, rotation);
                spawnedObject.transform.parent = parentObject.transform;

                foreach (Renderer rend in spawnedObject.GetComponentsInChildren<Renderer>())
                {
                    rend.sharedMaterial = rend.sharedMaterial;
                }

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

        Renderer terrainRenderer = terrain.GetComponent<Renderer>();

        minX = terrainRenderer.bounds.center.x - terrainRenderer.bounds.extents.x;
        maxX = terrainRenderer.bounds.center.x + terrainRenderer.bounds.extents.x;

        minZ = terrainRenderer.bounds.center.z - terrainRenderer.bounds.extents.z;
        maxZ = terrainRenderer.bounds.center.z + terrainRenderer.bounds.extents.z;


        if (GameManager.Instance.randomSeed == 0)
        {
            GameManager.Instance.randomSeed = System.DateTime.Now.Millisecond;
        }
        Random.InitState(GameManager.Instance.randomSeed);
        for (int i = 0; i < objects.Length; i++)
        {
            SpawnObject(objects[i], amounts[i]);
        }
        Debug.Log($"Random Seed: {GameManager.Instance.randomSeed}");

    }

}
