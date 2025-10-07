using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class FootstepPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] walkingSFX;
    public AudioClip[] runningSFX;
    public AudioClip[] walkingSandSFX;
    public AudioClip[] runningSandSFX;

    public float minVol;
    public float maxVol;

    public float rayDistance = 1f; // distance to check below the player

    public void PlayFootstep(int animation)
    {
        // Determine what type of surface is under the player
        AudioClip[] footstep;
        bool onSand = IsOnSand();

        if (animation == 1) // running
            footstep = onSand ? runningSandSFX : runningSFX;
        else // walking
            footstep = onSand ? walkingSandSFX : walkingSFX;

        // Pick random sound and volume
        int i = Random.Range(0, footstep.Length);
        AudioClip randomSound = footstep[i];
        float volume = Random.Range(minVol, maxVol);

        audioSource.PlayOneShot(randomSound, volume);
    }

    private bool IsOnSand()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Sand"))
                return true;
        }
        return false;
    }

}
