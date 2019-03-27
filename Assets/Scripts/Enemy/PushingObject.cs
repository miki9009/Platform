using Engine;
using System.Collections;
using UnityEngine;

public class PushingObject : LevelElement
{
    public float speed = 3;
    public float forwardFactor = 6;
    public float waitTime = 1;
    public float startWaitTime = 1;

    float curWaitTime;
    Vector3 startPos;
    Vector3 endPos;
    float anim = 0;
    public bool forward;
    public ParticleSystem smoke;

    private void Start()
    {
        endPos = transform.position - transform.forward * forwardFactor;
        startPos = transform.position;
        curWaitTime = waitTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!forward) return;
        var characterMovement = collision.collider.GetComponent<CharacterMovement>();
        if (characterMovement != null)
        {
            characterMovement.enabled = false;
            characterMovement.Hit(null, 100, true);
            characterMovement.rb.AddForce(transform.forward * 100, ForceMode.VelocityChange);
        }
    }

    private void Update()
    {
        if(startWaitTime > 0)
        {
            startWaitTime -= Time.deltaTime;
            return;
        }
        if(forward)
        {
            if (anim < 1)
            {
                anim += Time.deltaTime * speed;
            }
            else
            {
                forward = false;
                anim = 1;
                smoke.Play();
            }
        }
        else
        {
            if(anim > 0)
            {
                anim -= Time.deltaTime * speed;
            }
            else
            {
                if (curWaitTime < waitTime)
                {
                    curWaitTime += Time.deltaTime;
                }
                else
                {
                    forward = true;
                    anim = 0;
                    curWaitTime = 0;
                }
            }
        }
        transform.position = Vector3.Slerp(startPos, endPos, anim);
    }

    public override void OnSave()
    {
        base.OnSave();
        if(data!=null)
        {
            data["Speed"] = speed;
            data["ForwardFactor"] = forwardFactor;
            data["Forward"] = forward;
            data["WaitTime"] = waitTime;
            data["StartWaitTime"] = startWaitTime;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            speed = (float)data["Speed"];
            forwardFactor = (float)data["ForwardFactor"];
            forward = (bool)data["Forward"];
            waitTime = (float)data["WaitTime"];
            startWaitTime = (float) data["StartWaitTime"];
        }
    }

}