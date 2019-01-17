using System.Collections;
using UnityEngine;

public class PushingObject : MonoBehaviour
{
    public float speed = 1;
    public float forwardFactor = 10;
    public float waitTime = 1;

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
        if(characterMovement!= null)
        {
            characterMovement.enabled = false;
            characterMovement.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
            characterMovement.rb.velocity = Vector3.forward * 5;
            Controller.Instance.gameCamera.GetComponent<GameCamera>().target = null;
            characterMovement.anim.Play("Die");
            StartCoroutine(MoveCoroutine(characterMovement.transform, -Vector3.forward));
        }
    }

    IEnumerator MoveCoroutine(Transform trans, Vector3 dir)
    {
        while (!Controller.Instance.IsRestarting)
        {
            trans.position += dir * speed *5f * Time.deltaTime;
            yield return null;
        }
    }


    private void Update()
    {
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

}