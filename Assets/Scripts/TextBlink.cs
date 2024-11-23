using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TextBlink : MonoBehaviour
{
    public TMP_Text blinkingText;
    public float blinkDuration = 2f;
    public float fadeSpeed = 1f;

    private void Start()
    {
        if (blinkingText == null)
        {
            blinkingText = GetComponent<TMP_Text>();
        }

        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        Color originalColor = blinkingText.color;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            float alpha = Mathf.PingPong(elapsedTime * fadeSpeed, 1f) * 180f / 255f;
            blinkingText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set alpha to 0 at the end
        blinkingText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}