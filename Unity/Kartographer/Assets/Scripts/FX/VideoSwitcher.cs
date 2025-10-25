using UnityEngine;
using UnityEngine.Video;

public class VideoSwitcher : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip clip1;
    public VideoClip clip2;
    public VideoClip scareClip;

    void Start()
    {
        videoPlayer.clip = clip1;
        videoPlayer.Play();
    }

    public void SwitchToClip2()
    {
        videoPlayer.clip = clip2;
        videoPlayer.Play();
    }

    public void SwitchToScareClip()
    {
        videoPlayer.clip = scareClip;
        videoPlayer.Play();
    }
}
