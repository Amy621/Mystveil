using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour{
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] public float fadeDuration = 5.0f;
    [SerializeField] public bool fadeIn = false;

    public void Start()
    {
        if(fadeIn)
            FadeIn();

    }

    public void FadeIn(){
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
    }
    public void FadeOut(){
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration){
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }
    public IEnumerator FadeOutThenLoad()
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
        SceneManager.LoadScene("Town 1");
    }
}

