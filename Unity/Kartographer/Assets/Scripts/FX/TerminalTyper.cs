using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TerminalTyper : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] keyboardSounds;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 85f; // Characters per second
    [SerializeField] private float lineDelay = 0.05f; // Delay between lines
    [SerializeField] private int soundFrequency = 7; // Play sound every N characters
    [SerializeField] private float soundVolume = 0.07f;

    /// <summary>
    /// Types out lines of text with typewriter effect. Use ! for color tags (toggles red text).
    /// </summary>
    /// <param name="textComponent">The TMP_Text component to type into</param>
    /// <param name="lines">Array of strings to type out</param>
    /// <param name="colorHex">Hex color for text between ! markers (default: #960019)</param>
    public IEnumerator TypeLines(TMP_Text textComponent, string[] lines, Image bgImage, string colorHex = "#960019")
    {
        yield return FadeRoutine(true, 0.1f, bgImage);
        textComponent.enabled = true;
        textComponent.text = "";
        int rand = Random.Range(0, keyboardSounds.Length);
        bool inColorTag = false;
        int charCount = 0;

        foreach (string line in lines)
        {
            foreach (char c in line)
            {
                if (c == '!')
                {
                    if (!inColorTag)
                    {
                        textComponent.text += $"<color={colorHex}>";
                        inColorTag = true;
                    }
                    else
                    {
                        textComponent.text += "</color>";
                        inColorTag = false;
                    }
                }
                else
                {
                    textComponent.text += c;
                    charCount++;

                    if (charCount % soundFrequency == 0 && keyboardSounds.Length > 0)
                    {
                        SoundManager.Instance.PlaySound2D(keyboardSounds[rand], "SFX", soundVolume, true);
                    }

                    yield return new WaitForSeconds(1f / typeSpeed);
                }
            }

            textComponent.text += "\n";
            yield return new WaitForSeconds(lineDelay);
        }

        if (inColorTag)
            textComponent.text += "</color>";

        yield return FadeRoutine(false, 3f, bgImage);
        textComponent.enabled = false;

    }

    private IEnumerator FadeRoutine(bool fadeIn, float duration, Image bgImage)
    {
        float time = 0f;
        Color start = bgImage.color;
        Color end = bgImage.color;
        end.a = fadeIn ? 1f : 0f;

        while (time < duration)
        {
            bgImage.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        bgImage.color = end;
    }
}