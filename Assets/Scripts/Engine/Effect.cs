using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Effect : MonoBehaviour
{
    public enum Type { Slide, Scale, Rotation, Alpha }
    public float duration = 1;
    public Animation[] animations;
    public event Action PlayFinished;

    float progress = 0;
    public float Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = Mathf.Clamp01(value);
        }
    }

    [Serializable]
    public class Animation
    {
        public Type type;
        public AnimationCurve curve;
    }

    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play()
    {
        if(activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        StartCoroutine(PlayCoroutine());
    }

    Coroutine activeCoroutine;
    IEnumerator PlayCoroutine()
    {
        Vector3 startScale = transform.localScale;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            for (int i = 0; i < animations.Length; i++)
            {
                if(animations[i].type == Type.Scale)
                {
                    transform.localScale = startScale * animations[i].curve.Evaluate(progress);
                }
                else if(animations[i].type == Type.Alpha)
                {
                    canvasGroup.alpha = animations[i].curve.Evaluate(progress);
                }

            }
            PlayFinished?. Invoke();
            yield return null;
        }
        progress = 0;
        activeCoroutine = null;
    }
}