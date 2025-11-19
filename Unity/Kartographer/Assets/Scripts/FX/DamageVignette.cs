using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageVignette : MonoBehaviour
{
    [SerializeField] private Image dmgImage;
    [SerializeField] private float fadeDuration = 0.5f;

    void Start()
    {
        if (dmgImage != null)
            dmgImage.enabled = false;
    }

    public IEnumerator ShowDMG()
    {
        if (dmgImage == null) yield break;

        dmgImage.enabled = true;

        Color color = dmgImage.color;
        color.a = 1f;
        dmgImage.color = color;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            dmgImage.color = color;
            yield return null;
        }

        color.a = 0f;
        dmgImage.color = color;
        dmgImage.enabled = false;
    }
}
