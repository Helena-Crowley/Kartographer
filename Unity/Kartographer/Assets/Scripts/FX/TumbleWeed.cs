using System.Collections;
using UnityEngine;

public class TumbleWeed : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minRandTime = 1f;
    [SerializeField] private float maxRandTime = 5f;

    private bool tumble = true;

    [SerializeField] private Vector3 windDirection = new Vector3(1, 0, 0);
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 15f;

    private void Start()
    {
        StartCoroutine(BlowWind());
    }

    private IEnumerator BlowWind()
    {
        while (true)
        {
            float waitTime = Random.Range(minRandTime, maxRandTime);
            yield return new WaitForSeconds(waitTime);

            if (tumble)
            {
                Vector3 randomForce = windDirection.normalized * Random.Range(minForce, maxForce);
                randomForce += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); // some random sway
                rb.AddForce(randomForce, ForceMode.Impulse);

                // Optional: add torque for rolling
                Vector3 randomTorque = new Vector3(Random.Range(-10f, 10f), Random.Range(-30f, 30f), Random.Range(-10f, 10f));
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
    }

}
