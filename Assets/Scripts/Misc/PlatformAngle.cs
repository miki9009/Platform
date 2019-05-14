using UnityEngine;

using Engine;

public class PlatformAngle : LevelElement
{
    public AnimationCurve curve;

    bool expand;
    public float animationSpeed = 1;
    public float delay;
    public float waitTime = 2;
    public Transform rotator;

    float time;
    float curWait;
    Vector3 startRotation = new Vector3(89, 90, -90);
    Vector3 endRotation = new Vector3(269, 90, -90);

    private void Update()
    {
        if(curWait > 0)
        {
            curWait -= Time.deltaTime;
            return;
        }
        else
        {
            if (expand)
            {
                time += Time.deltaTime * animationSpeed;
                if (time >= 1)
                {
                    expand = false;
                    curWait = waitTime;
                }
            }
            else
            {
                time -= Time.deltaTime * animationSpeed;
                if (time <= 0)
                {
                    expand = true;
                    curWait = waitTime;
                }
            }
            rotator.transform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, curve.Evaluate(time));
        }
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