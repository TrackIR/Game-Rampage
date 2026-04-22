using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class TutorialTextFade : MonoBehaviour
{
     public CanvasGroup canvasGroup;
    public float fadeDuration = 0.75f;
    private TMP_Text tutorialText;

    void Start()
    {
        tutorialText = GetComponentInChildren<TMP_Text>();
    }

    public void FadeIn(String text)
    {
        tutorialText.text = text;
        StopAllCoroutines();
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float start, float end)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        canvasGroup.alpha = end;
    }

}
