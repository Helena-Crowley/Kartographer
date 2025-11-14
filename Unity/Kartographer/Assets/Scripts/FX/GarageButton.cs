using UnityEngine;

public class GarageButton : MonoBehaviour
{
    [SerializeField] private Vector3 buttonPressedPos;
    [SerializeField] private Vector3 buttonRestPos;
    [SerializeField] private Material buttonMaterial;
    [SerializeField] private float intensity = 1.75f;
    [SerializeField] private AudioClip buttonPressed;
    [SerializeField] private AudioClip buttonUnPressed;

    private bool isPressed = false;
    private Color baseColor;

    void Start()
    {
        baseColor = buttonMaterial.color;
        transform.localPosition = buttonRestPos;
    }

    public void PressButton()
    {
        if (isPressed) return;

        isPressed = true;
        SoundManager.Instance.PlaySound(buttonPressed, transform.position, "SFX", 0.2f);
        buttonMaterial.color = baseColor;
        transform.localPosition = buttonPressedPos;
    }

    public void UnpressButton()
    {
        if (!isPressed) return;
        isPressed = false;
        transform.localPosition = buttonRestPos;
        SoundManager.Instance.PlaySound(buttonUnPressed, transform.position, "SFX", 0.2f);
        buttonMaterial.color = Color.limeGreen;
    }
}
