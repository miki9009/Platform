using Engine;
using System.Collections;
using UnityEngine;

public class Spikes : LevelElement
{
    public AnimationCurve curve;
    public AnimationCurve curveDown;
    public Transform spikes;
    public float yMin = 0;
    public float yMax = 1;
    public float animationSpeed = 1;
    public float timeSpikesUp = 2;
    public float timeSpikesDown = 2;
    public float startWaitTime = 2;
    public bool active = false;
    public BoxCollider boxCollider;

    float time;
    Vector3 downSpikes;
    Vector3 upSpikes;

    public bool AreUp { get; private set; }

    public override void OnSave()
    {
        base.OnSave();
        data["Ymin"] = yMin;
        data["Ymax"] = yMax;
        data["animationSpeed"] = animationSpeed;
        data["timeSpikesUp"] = timeSpikesUp;
        data["timeSpikesDown"] = timeSpikesDown;
        data["startWaitTime"] = startWaitTime;
        data["active"] = active;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (!data.ContainsKey("Ymin")) return;
        yMin = (float)data["Ymin"];
        yMax = (float)data["Ymax"];
        animationSpeed = (float)data["animationSpeed"];
        timeSpikesUp = (float)data["timeSpikesUp"];
        timeSpikesDown = (float)data["timeSpikesDown"];
        startWaitTime = (float)data["startWaitTime"];
        active = (bool)data["active"];
    }

    public override void ElementStart()
    {
        base.ElementStart();
        //StartCoroutine(MainUpdate());
        downSpikes = new Vector3(0, yMin, 0);
        upSpikes = new Vector3(0, yMax, 0);

        if (active)
        {
            spikes.transform.localPosition =downSpikes;
            StartCoroutine(SpikesBody());
        }
        else
        {
            AreUp = true;
            spikes.transform.localPosition = upSpikes;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!AreUp) return;
        if(other.gameObject.layer == Layers.Character)
        {
            var character = other.GetComponent<CharacterMovement>();
            if(!character.character.IsDead)
                character.Hit(null, 10000, true);
        }
    }

    IEnumerator SpikesBody()
    {
        float time = 0;
        while (time < startWaitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        while (true)
        {
            time = 0;
            AreUp = true;
            while (time < 1)
            {
                time += Time.deltaTime * animationSpeed;
                spikes.transform.localPosition = Vector3.Lerp(downSpikes, upSpikes, curve.Evaluate(time));
                yield return null;
            }

            time = 0;
            while(time < timeSpikesUp)
            {
                time += Time.deltaTime;
                yield return null;
            }
            AreUp = false;
            time = 0;

            while (time < 1)
            {
                time += Time.deltaTime * animationSpeed;
                spikes.transform.localPosition = Vector3.Lerp(downSpikes, upSpikes, curveDown.Evaluate(time));
                yield return null;
            }


            time = 0;
            while (time < timeSpikesDown)
            {
                time += Time.deltaTime;
                yield return null;
            }


        }
    }
}