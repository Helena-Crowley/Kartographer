using UnityEngine.UI;
using UnityEngine;

public class LightUIIcon : MonoBehaviour
{
    private CarHeadlight carHeadlight;
    public Image lightIcon;

    public Sprite lightOnIcon;
    public Sprite lightOffIcon;

    public void updateUI(GameObject cart, bool lightsOn)
    {
        if (lightsOn) carHeadlight = cart.GetComponent<CarHeadlight>();
        else if (!lightsOn) carHeadlight = null;
    }


    // Update is called once per frame
    void Update()
    {
        if (carHeadlight != null)
        {
            if (carHeadlight.headlightsOn)
            { lightIcon.sprite = lightOnIcon; }
            else
            { lightIcon.sprite = lightOffIcon; }
        }
    }
}
