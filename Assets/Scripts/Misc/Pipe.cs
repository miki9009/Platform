using Engine;
using System.Collections;
using UnityEngine;


public class Pipe : LevelElement
{
    public float pipeForce;
    public int numberOfLeaves = 25;
    public bool blur = true;
    public ParticleSystem parts;
    //public ActivationTrigger trigger;
    public bool Activated{get;set;}
    public float shootSpeed = 1;
    public AnimationCurve animationCurve;

    public bool Used { get; set; }
    public BezierCurve curve;
    Vector3 currentCharacterPos;
    CharacterMovement characterMovement;
    Quaternion characterRotation;
    SphereCollider sphere;



    void Awake()
    {
        sphere = GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterMovement movement = other.GetComponentInParent<CharacterMovement>();
        if (movement == null) return;
        if (!Used)
        {
            StartCoroutine(JumpToPipeCor(movement));
        }
    }

    bool shoot = false;
    IEnumerator JumpToPipeCor(CharacterMovement movement)
    {

        var cam = Controller.Instance.gameCamera.GetComponent<GameCamera>();
        //float lastSpeed = cam.speed;
        //cam.speed = lastSpeed / 6;
        characterMovement = movement;



        Used = true;
        movement.SetAnimation("JumpUp");



        movement.MovementEnabled = false;
        yield return new WaitForSeconds(0.2f);
        if (curve == null)
        {
            curve = GetComponentInChildren<BezierCurve>();
        }
        //while (!pipeEntered)
        //{
        //    var dis = Vector3.Distance(transform.position, currentCharacterPos);
        //    currentCharacterPos = movement.transform.position;
        //    characterRotation = movement.transform.rotation;
        //    Vector3 pipePos = new Vector3(transform.position.x, currentCharacterPos.y, transform.position.z);
        //    if (dis < 1)
        //    {
        //        pipeEntered = true;
        //    }
        //    if (animation < 1)
        //    {
        //        animation += Time.deltaTime * dis / 10;
        //    }
        //    if (Vector3.Distance(transform.position, pipePos) > 1)
        //    {
        //        characterRotation = Quaternion.Slerp(characterRotation, Quaternion.LookRotation(Vector.Direction(currentCharacterPos, pipePos + Vector3.forward)),Time.deltaTime * 10);
        //        currentCharacterPos = Vector3.Lerp(currentCharacterPos, pipePos, animation);
        //    }

        //    yield return null;
        //}
        //parts.Emit(numberOfLeaves);
        float progress = 0;
        cam.motionBlure.enabled = blur;

        Vector3 aim = points[1].position;
        Debug.Log("AIM: " + aim);
        //aim.y = currentCharacterPos.y;
        characterRotation = movement.transform.rotation;
        Quaternion destinationRotation = Quaternion.LookRotation(Vector.Direction(movement.transform.position, points[1].position));
        //float camProgress = 0;
        float smoothProgress = 0;

        while (progress < 0.8f)
        {   
            characterRotation = Quaternion.Slerp(characterRotation, destinationRotation, Time.deltaTime * 10);
            progress += Time.deltaTime / 2;
            //cam.CameraLookAtPosition(Vector3.Lerp(camStartPos, aim, camProgress));
            //camProgress = Mathf.Clamp01(progress * 2);
            smoothProgress = animationCurve.Evaluate(progress);
            cam.motionBlure.blurAmount = 1 - smoothProgress;
            currentCharacterPos = curve.GetPointAt(smoothProgress);
            if (!shoot)
                shoot = true;
            yield return null;
        }
        //characterMovement.transform.position = points[1].position;
        cam.motionBlure.enabled = false;
        //movement.rb.velocity = Vector3.zero;
        Used = false;
        movement.MovementEnabled = true;
        //cam.SetTarget(target);
        //cam.speed = lastSpeed;
        shoot = false;
        yield return new WaitForSeconds(0.1f);
        //cam.SetTarget(target);
    }

    private void FixedUpdate()
    {
        if(shoot && characterMovement)
        {
            characterMovement.transform.position = Vector3.Lerp(characterMovement.transform.position, currentCharacterPos, 0.1f);
            var pos = points[1].position;
            characterMovement.transform.rotation = Quaternion.LookRotation(new Vector3(pos.x,characterMovement.transform.position.y,pos.z) - characterMovement.transform.position);
        }

    }



    public Transform[] points;
    Float3[] pointsPos;
    Float4[] pointsRot;
    Float3[] handle1Pos;
    Float3[] handle2Pos;

    public override void OnSave()
    {
        base.OnSave();
        int length = points.Length;
        pointsPos = new Float3[length];
        pointsRot = new Float4[length];
        handle1Pos = new Float3[length];
        handle2Pos = new Float3[length];
        for (int i = 0; i < length; i++)
        {
            pointsPos[i] = points[i].localPosition;
            pointsRot[i] = points[i].localRotation;
            var handle = points[i].GetComponent<BezierPoint>();
            handle1Pos[i] = handle.handle1;
            handle2Pos[i] = handle.handle2;
        }
        if (data != null)
        {
            data["Points"] = pointsPos;
            data["Rotations"] = pointsRot;
            data["Handle1"] = handle1Pos;
            data["Handle2"] = handle2Pos;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            if (data.ContainsKey("Points"))
                pointsPos = (Float3[])data["Points"];
            if (data.ContainsKey("Rotations"))
                pointsRot = (Float4[])data["Rotations"];
            if (data.ContainsKey("Handle1"))
                handle1Pos = (Float3[])data["Handle1"];
            if (data.ContainsKey("Handle2"))
                handle2Pos = (Float3[])data["Handle2"];
            for (int i = 0; i < pointsPos.Length; i++)
            {
                points[i].transform.localPosition = pointsPos[i];
                points[i].transform.localRotation = pointsRot[i];
                var handle = points[i].GetComponent<BezierPoint>();
                handle.handle1 = handle1Pos[i];
                handle.handle2 = handle2Pos[i];
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(points[1])
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(points[1].position, 1);
        }
    }
}