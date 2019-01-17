using System.Collections;
using UnityEngine;

public class ChangeDirection : MonoBehaviour
{
//    public BezierCurve curve;
//    public Transform[] points;
//    public Color gizmosColor = Color.white;
//    public TriggerBroadcast triggerBroadcastStart;
//    public ParticleSystem parts;
//    public Color startColor = Color.white;
//    public Color triggerColor = Color.white;

//    float speed = 1;
//    public float colorLerpSpeed = 1;
//    public bool reverseDir;
//    CharacterMovement characterMovement;
//    IEnumerator currentCoroutine;


//    private void Awake()
//    {
//        triggerBroadcastStart.TriggerEntered += StartMovement;
//        triggerBroadcastStart.TriggerExit += StartMovementExit;
//        speed = 1/Vector3.Distance(points[0].position, points[points.Length - 1].position);
//    }

//    void StartMovement(Collider coll)
//    {
//        Debug.Log("End Movement");
//        characterMovement = coll.GetComponentInParent<CharacterMovement>();
//        StartCoroutine( LerpColor(startColor, triggerColor));
//        if (characterMovement != null)
//        {
//            if(!reverseDir)
//                characterMovement.MoveUp += Move;
//            else
//                characterMovement.MoveDown += Move;
//            currentCoroutine = StartMoveC();
//        }
//    }

//    void StartMovementExit(Collider coll)
//    {
//        Debug.Log("Start Movement");
//        StartCoroutine(LerpColor(triggerColor, startColor));
//        if(characterMovement != null)
//        {
//            if (!reverseDir)
//                characterMovement.MoveUp -= Move;
//            else
//                characterMovement.MoveDown -= Move;
//            currentCoroutine = null;
//        }
//    }


//    public void Move(CharacterMovement characterMovement)
//    {
//        if (currentCoroutine != null)
//            StartCoroutine(currentCoroutine);
//        else
//            Debug.LogError("Current coroutine was null, this shouldn't happen");
//    }

//    IEnumerator LerpColor(Color startC, Color endC)
//    {
//        float t = 0;
//        while(t < 1)
//        {
//            t += Time.deltaTime * colorLerpSpeed;
//            parts.startColor = Color.Lerp(startC, endC, t);
//            yield return null;
//        }
//    }

//    IEnumerator StartMoveC()
//    {
//        Debug.Log("StartMove");
//        if (characterMovement == null)
//        {
//            Debug.LogError("Character movement is null");
//            yield break;
//        }
//        characterMovement.enabled = false;
//        characterMovement.anim.Play("Idle");

//        float move = 0;
//        Vector3 startPos = characterMovement.transform.position;
//        while(move < 1)
//        {
//            move += 0.05f;
//            var pos = curve.GetPointAt(0);
//            characterMovement.transform.position = Vector3.Lerp(startPos, new Vector3(pos.x,characterMovement.transform.position.y, pos.z) , move);
//            yield return null;
//        }

//        float turn = 0;
//        Quaternion startRot = characterMovement.transform.rotation;
//        while (turn < 1f)
//        {
//            turn += 0.1f;
//            characterMovement.transform.rotation = Quaternion.Lerp(startRot, Quaternion.LookRotation(Engine.Vector.Direction(characterMovement.transform.position, points[points.Length - 1].position)),turn);
//            yield return null;
//        }
//        characterMovement.anim.Play("Run");
//        characterMovement.anim.SetFloat("hSpeed", 8f);
//        characterMovement.transform.rotation = Quaternion.LookRotation(Engine.Vector.Direction(characterMovement.transform.position, points[points.Length - 1].position));
//        float time = 0;
//        while(time < 1)
//        {
//            time += Time.deltaTime * speed * 7.5f;
//            var pos = curve.GetPointAt(time);
//            characterMovement.transform.position = new Vector3(pos.x, characterMovement.transform.position.y,pos.z);
//            yield return null;
//        }
//        turn = 0;
//        Quaternion endRot = characterMovement.transform.rotation;
//        while (turn < 1f)
//        {
//            turn += 0.1f;
//            characterMovement.transform.rotation = Quaternion.Lerp(endRot, startRot, turn);
//            yield return null;
//        }
//        characterMovement.enabled = true;
//    }

//    IEnumerator EndMoveC()
//    {
//        Debug.Log("EndMove");
//        if (characterMovement == null)
//        {
//            Debug.LogError("Character movement is null");
//            yield break;
//        }
//        float time = 1;
//        while (time > 0)
//        {
//            time -= Time.deltaTime * speed;
//            var pos = curve.GetPointAt(time);
//            characterMovement.transform.position = new Vector3(characterMovement.transform.position.x, characterMovement.transform.position.y, pos.z);
//            yield return null;
//        }
//    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = gizmosColor;
//        for (int i = 0; i < points.Length; i++)
//        {
//            if(points[i]!=null)
//                Gizmos.DrawSphere(points[i].position, 2);
//        }
//    }
//#endif
}