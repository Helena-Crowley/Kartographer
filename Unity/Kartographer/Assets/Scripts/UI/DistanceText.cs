using UnityEngine;

public class DistanceText : MonoBehaviour
{
    public TMPro.TMP_Text distanceText;
    public Transform playerTransform;
    public Transform stormTransform;

    private float timer = 0f;    
    private float updateInterval = 0.5f;

    void Start()
    {
        distanceText.text = (playerTransform.position.x - stormTransform.position.x).ToString("F1") + "m";
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            distanceText.text = (playerTransform.position.x - stormTransform.position.x).ToString("F1") + "m";
            timer = 0f;
        }
    }
}
