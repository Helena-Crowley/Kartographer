using UnityEngine;

public class GarageButton : MonoBehaviour
{
    [SerializeField] private Vector3 buttonDepressedPos;
    [SerializeField] private Vector3 buttonRestPos;
    [SerializeField] private Material buttonMaterial;
    [SerializeField] private float intensity = 1.75f;

    private bool isPressed = false;
    private Color baseColor;

    void Start()
    {
        baseColor = buttonMaterial.color;
    }

    public void PressButton()
    {
        if (isPressed) return;

        isPressed = true;
        buttonMaterial.EnableKeyword("_EMISSION");

        buttonMaterial.SetColor("_EmissionColor", baseColor);
        transform.localPosition = buttonDepressedPos;
    }

    public void UnpressButton()
    {
        isPressed = false;
        transform.localPosition = buttonRestPos;

        buttonMaterial.EnableKeyword("_EMISSION");

        buttonMaterial.SetColor("_EmissionColor", baseColor * intensity);
    }
}
