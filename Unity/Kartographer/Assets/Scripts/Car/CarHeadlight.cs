using UnityEngine;
using UnityEngine.InputSystem;

public class CarHeadlight : MonoBehaviour
{
    public GameObject headlightLeft;
    public GameObject headlightRight;

    [HideInInspector]
    public bool headlightsOn = false;

    [SerializeField] private InputActionReference toggleHeadlightAction;

    // Update is called once per frame
    void Update()
    {
        if (toggleHeadlightAction.action.WasPressedThisFrame())
        {
            bool newState = !headlightLeft.activeSelf;
            headlightsOn = newState;
            headlightLeft.SetActive(newState);
            headlightRight.SetActive(newState);
        }
    }
}
