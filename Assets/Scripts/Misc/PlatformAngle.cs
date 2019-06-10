using UnityEngine;

using Engine;
using System.Collections;

public class PlatformAngle : LevelElement
{
    public AnimationCurve curve;

    static event System.Action Triggered;
    bool expand;
    public float animationSpeed = 1;
    public float delay;
    public float waitTime = 2;
    public Transform rotator;

    float time;
    float curWait;
    Vector3 startRotation = new Vector3(89, 90, -90);
    Vector3 endRotation = new Vector3(269, 90, -90);

    public override void ElementStart()
    {
        base.ElementStart();
        Triggered += OnTriggered;
    }

    private void OnDestroy()
    {
        Triggered -= OnTriggered;
    }

    bool activated;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        activated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(activated)
        {
            Triggered?.Invoke();
        }
        activated = false;
    }

    void OnTriggered()
    {
        if(coroutine == null)
        {
            coroutine = ChangeMovement();
            time = 0;
            StartCoroutine(coroutine);
        }
    }

    IEnumerator coroutine;
    IEnumerator ChangeMovement()
    {
        expand = !expand;
        while ( time < 1)
        {
            time += Time.deltaTime * animationSpeed;
            if (expand)
            {
                rotator.transform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, curve.Evaluate(time));
            }
            else
            {
                rotator.transform.localEulerAngles = Vector3.Lerp(endRotation, startRotation, curve.Evaluate(time));

            }
            yield return null;
        }

        coroutine = null;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("AnimationSpeed"))
            animationSpeed = (float)data["AnimationSpeed"];
        if (data.ContainsKey("DelayTime"))
            delay = (float)data["DelayTime"];
        if (data.ContainsKey("WaitTime"))
            delay = (float)data["WaitTime"];

        curWait = delay;
    }

    public override void OnSave()
    {
        base.OnSave();
        data["AnimationSpeed"] = animationSpeed;
        data["DelayTime"] = delay;
        data["WaitTime"] = waitTime;
    }
}