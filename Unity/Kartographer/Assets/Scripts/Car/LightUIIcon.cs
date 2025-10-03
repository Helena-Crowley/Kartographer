using UnityEngine.UI;
using UnityEngine;

public class LightUIIcon : MonoBehaviour
{
    public CarHeadlight carHeadlight;
    public Image lightIcon;

    public Sprite lightOnIcon;
    public Sprite lightOffIcon;

    // Update is called once per frame
    void Update()
    {
        if (carHeadlight.headlightsOn)
        { lightIcon.sprite = lightOnIcon; }
        else
        { lightIcon.sprite = lightOffIcon; }
    }
}
