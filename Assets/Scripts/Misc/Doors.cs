using Engine;
using System.Collections;
using UnityEngine;

public class Doors : LevelElement, IActivationTrigger
{
    public bool Activated { get; set; }
    SphereCollider sphere;
    public bool Used { get; set; }
    bool opened = false;
    public Transform padlock;
    public Transform door1;
    public Transform door2;
    public Vector3 door1EndRotation;
    public Vector3 door2EndRotation;
    public float animationSpeed = 1;

    protected void Awake()
    {
        sphere = GetComponent<SphereCollider>();
    }

    public void Activate()
    {
        if (Activated && !opened && Vector3.Distance(transform.position, Character.GetLocalPlayer().transform.position) < sphere.radius)
        {
            int val = CollectionManager.Instance.GetCollection(Character.GetLocalPlayer().ID, CollectionType.KeySilver);
            if (val > 0)
            {
                CollectionManager.Instance.AddToCollection(Character.GetLocalPlayer().ID, CollectionType.KeySilver,  -1);
                opened = true;
                if (enabled)
                    StartCoroutine(OpenDoors());
                Used = true;
                ActivationTrigger.activatedTriggers -= ActivationTrigger.CHARACTER_TRIGGERS;
                if (ActivationTrigger.activatedTriggers <= 0)
                {
                    GetComponent<ActivationTrigger>().DeactivateTrigger();
                }
            }
            else
            {
                if(shakeCoroutine == null)
                    shakeCoroutine = StartCoroutine(Shake());
            }

        }
    }

    IEnumerator OpenDoors()
    {
        padlock.gameObject.SetActive(false);
        float progress = 0;
        
        while(progress < 1)
        {
            progress += animationSpeed * Time.deltaTime;
            door1.localRotation = Quaternion.Lerp(door1.localRotation, Quaternion.Euler(door1EndRotation), progress);
            door2.localRotation = Quaternion.Lerp(door2.localRotation, Quaternion.Euler(door2EndRotation), progress);
            yield return null;
        }
        yield return null;
    }

    public float padlokShakeSpeed = 1;
    public int numerOfShakes = 5;
    Coroutine shakeCoroutine;
    IEnumerator Shake()
    {

        Vector3 startRot = padlock.localEulerAngles;
        for (int i = 0; i < numerOfShakes; i++)
        {
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * padlokShakeSpeed;
                padlock.localRotation = Quaternion.Slerp(padlock.localRotation, Quaternion.Euler(i % 2 == 0 ? -30 : startRot.x, startRot.y, i % 2 == 0 ? -45 : 45),progress);
                yield return null;
            }
        }
        padlock.localRotation = Quaternion.Euler(startRot);
        shakeCoroutine = null;
    }

    public bool debug;
    void OnDrawGizmos()
    {
        if(debug)
        {
            if(shakeCoroutine==null)
                shakeCoroutine=StartCoroutine(Shake());
            debug = false;
        }
    }
}