using Engine;
using Engine.UI;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ActivationTrigger : MonoBehaviour
{
    public static bool isActivated;
    IActivationTrigger component;
    public static int activatedTriggers = 0;
    int thisActivators = 0;
    Button button;
    SphereCollider col;
    public Character Character { get; set; }

    public const int CHARACTER_TRIGGERS = 1;

    static ActivationTrigger host;


    private void OnTriggerEnter(Collider other)
    {
        activatedTriggers++;
        if (component.Activated || component.Used) return;
        component.Activated = true;
        if (button != null)
        {
            if (host != null) host.StopAllCoroutines();
            host = this;
            ActivateButton(true);
        }
        //Debug.Log("Activated " + transform.name);
    }


    public void OnTriggerExit(Collider other)
    {
        activatedTriggers--;
        if (activatedTriggers <= 0)
        {
            if (host != null) host.StopAllCoroutines();
            host = this;
            ActivateButton(false);
        }
        component.Activated = false;

        //Debug.Log("Disabled " + transform.name);
    }

    public void DeactivateTrigger()
    {
        col.enabled = false;
        GameGUI.GetButtonByName("Action").OnTapPressed.RemoveListener(component.Activate);
        component.Activated = false;
        if (activatedTriggers <= 0)
        {
            ActivateButton(false);
        }
    }


    private void Start()
    {
        button = GameGUI.GetButtonByName("Action");
        component = GetComponent<IActivationTrigger>();
        if (component != null)
        {
            try
            {
                button.OnTapPressed.AddListener(component.Activate);
            }
            catch { }

        }
        col = GetComponent<SphereCollider>();
    }

    void ActivateButton(bool set)
    {       
        if (set)
        {
            Expand(button.transform, 2, 1, () => { button.transform.localScale = Vector3.one; host = null; }, () => button.transform.localScale = Vector3.one);
            button.gameObject.SetActive(set);
        }
        else
        {
            Shrink(button.transform, 2, 1, () => { button.gameObject.SetActive(set); button.transform.localScale = Vector3.one; host = null; });
        }
    }

    private void OnDestroy()
    {
        if (component != null)
        {
           var button = GameGUI.GetButtonByName("Action");
            if (button != null)
            {
                button.OnTapPressed.RemoveListener(component.Activate);
            }
        }
    }

    void Expand(Transform transform, float animationSpeed, int loop, System.Action onFinished = null, System.Action onStart = null)
    {
        if (onStart != null)
        {
            onStart();
        }
        StopAllCoroutines();
        StartCoroutine(ScaleEffectC(transform, animationSpeed, loop, onFinished));
    }

    void Shrink(Transform transform, float animationSpeed, int loop, System.Action onRemove = null)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleEffectVanishC(transform, animationSpeed, loop, onRemove));
    }

    IEnumerator ScaleEffectC(Transform transform, float animationSpeed, int loop, System.Action onFinished)
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
    IEnumerator ScaleEffectVanishC(Transform transform, float animationSpeed, int loop, System.Action onRemove)
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
}

public interface IActivationTrigger
{
    bool Activated { get; set; }
    bool Used { get; set; }
    void Activate();
}

