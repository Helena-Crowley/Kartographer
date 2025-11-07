using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class ContainerDoors : NetworkBehaviour
{
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;

    [SerializeField] private AudioClip doorSound;

    [SerializeField] private float openAngle = 100f;

    private bool hasOpened = false;


    //Testing
    //void Start() => OpenContainerDoors(4);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasOpened)
        {
            OpenContainerDoors((int)doorSound.length);
            hasOpened = true;
        }
    }


    /// <summary>
    /// Opens container doors after n seconds
    /// </summary>
    public void OpenContainerDoors(int openDuration)
    {
        StartCoroutine(OpenDoors(openDuration));
    }

    private IEnumerator OpenDoors(int openDuration)
    {
        Quaternion door1StartRot = door1.transform.localRotation;
        Quaternion door2StartRot = door2.transform.localRotation;

        Quaternion door1EndRot = door1StartRot * Quaternion.Euler(0f, 0f, openAngle);
        Quaternion door2EndRot = door2StartRot * Quaternion.Euler(0f, 0f, -openAngle);

        float elapsed = 0f;
        SoundManager.Instance.PlaySound(doorSound, door1.transform.position, 0.4f, true);
        SoundManager.Instance.PlaySound(doorSound, door2.transform.position, 0.4f, true);
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);

            door1.transform.localRotation = Quaternion.Slerp(door1StartRot, door1EndRot, t);
            door2.transform.localRotation = Quaternion.Slerp(door2StartRot, door2EndRot, t);

            yield return null;
        }

        door1.transform.localRotation = door1EndRot;
        door2.transform.localRotation = door2EndRot;
    }
}
