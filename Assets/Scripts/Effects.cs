using UnityEngine;
using System.Collections;
using System;

public class Effects : MonoBehaviour
{
    private static Effects instance;

    static Effects Instance
    {
        get
        {
            if (instance == null)
            {
                return instance = new GameObject("Effects").AddComponent<Effects>();
            }
            else
            {
                return instance;
            }
        }
    }

    public static void ScalePulse(Transform transform, float animationSpeed, int loop, Action onFinished = null, Action onStart = null)
    {
        if (onStart != null)
        {
            onStart();
        }
        Instance.StartCoroutine(Instance.ScaleEffectC(transform, animationSpeed, loop, onFinished));
    }

    public static void ScalePulseVanish(Transform transform, float animationSpeed, int loop, Action onRemove = null)
    {
        Instance.StartCoroutine(Instance.ScaleEffectVanishC(transform, animationSpeed, loop, onRemove));
    }

    IEnumerator ScaleEffectC(Transform transform, float animationSpeed, int loop, Action onFinished)
    {
        while (loop > 0)
        {
            float curScale = 2;
            Vector3 curVector = transform.localScale;
            Vector3 startVector = curVector;
            transform.localScale = curVector * 2;

            while (curScale > (1.05f))
            {
                transform.localScale = startVector * curScale;
                curScale -= Mathf.Pow(curScale, 1) * Time.deltaTime * animationSpeed;
                yield return null;
            }
            transform.localScale = startVector;
            loop--;
            yield return null;
        }
        if (onFinished != null)
        {
            onFinished();
        }
        else
        {
            Debug.Log("Finished was null");
        }
        yield return null;
    }
    IEnumerator ScaleEffectVanishC(Transform transform, float animationSpeed, int loop, Action onRemove)
    {
        Vector3 scale = transform.localScale;
        float curScale = 0;
        Vector3 startVector = default(Vector3);
        Vector3 curVector = default(Vector3);
        while (loop > 0)
        {
            curScale = 2;
            curVector = transform.localScale;
            startVector = curVector;
            transform.localScale = curVector * 2;

            while (curScale > (1.05f))
            {
                transform.localScale = startVector * curScale;
                curScale -= Mathf.Pow(curScale, 1) * Time.deltaTime * animationSpeed;
                yield return null;
            }
            transform.localScale = startVector;
            loop--;
            yield return null;
        }
        curScale = 1;

        while (curScale > 0.1f)
        {
            curScale -= 0.1f;
            transform.localScale = startVector * curScale;
            yield return null;
        }
        onRemove();
        transform.localScale = scale;
        yield return null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
