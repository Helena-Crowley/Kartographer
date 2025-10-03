using UnityEngine;

public class TrailerBehaviour : MonoBehaviour
{
    public Transform cartHitch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = cartHitch.position;        
    }
}
