using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float volume = 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound)
            SoundManager.Instance.PlaySound2D(hoverSound, "SFX",volume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound)
            SoundManager.Instance.PlaySound2D(clickSound, "SFX",volume);
    }
}
