using UnityEngine;
using UnityEngine.InputSystem;

public class CarHeadlight : MonoBehaviour
{
    public GameObject headlightLeft;
    public GameObject headlightRight;

    [HideInInspector]
    public bool headlightsOn = false;

    [SerializeField] private InputActionReference toggleHeadlightAction;
    [SerializeField] private LightUIIcon lightUIIcon;

    // Update is called once per frame
    void Update()
    {
        if (toggleHeadlightAction.action.WasPressedThisFrame())
        {
            bool newState = !headlightLeft.activeSelf;
            headlightsOn = newState;
            lightUIIcon.updateUI(gameObject, newState);
            headlightLeft.SetActive(newState);
            headlightRight.SetActive(newState);
        }
    }
}
