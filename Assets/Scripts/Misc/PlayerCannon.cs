using Engine;
using System.Collections;
using UnityEngine;


public class PlayerCannon: LevelElement, IActivationTrigger
{
    public float pipeForce;
    public int numberOfLeaves = 25;
    public bool blur = true;
    public ParticleSystem parts;
    //public ActivationTrigger trigger;
    public bool Activated { get; set; }
    public float shootSpeed = 1;
    public AnimationCurve animationCurve;
    public Transform barrelEnd;

    public bool Used { get; set; }
    public BezierCurve curve;
    Vector3 currentCharacterPos;
    CharacterMovement characterMovement;
    Quaternion characterRotation;
    SphereCollider sphere;
    public float radius = 6;
    //public ParticleSystem smokeParts;
    public ParticleSystem poof;
    public Transform insideBarrel;
    public Transform barrel;

    void Awake()
    {
        sphere = GetComponent<SphereCollider>();
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    CharacterMovement movement = other.GetComponentInParent<CharacterMovement>();
    //    if (movement == null) return;
    //    if (!Used)
    //    {
    //        StartCoroutine(JumpToPipeCor(movement));
    //    }
    //}

    bool shoot = false;
    IEnumerator JumpToPipeCor(CharacterMovement movement)
    {

        var cam = Controller.Instance.gameCamera.GetComponent<GameCamera>();

        characterMovement = movement;
        //smokeParts.Play();
        characterMovement.SetAnimation("Cannon");
        characterMovement.MovementEnabled = false;

        Used = true;
        poof.transform.position = characterMovement.transform.position + Vector3.up;
        poof.Play(true);
        float lerpFactor = 0;
        while (lerpFactor < 1)
        {
            lerpFactor += 0.1f;
            characterMovement.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, lerpFactor);
            yield return null;
        }
        characterMovement.transform.position = insideBarrel.position;
        //movement.rb.AddForce(Vector3.up * pipeForce, ForceMode.VelocityChange);
        //movement.SetAnimation("JumpUp");
        //movement.MovementEnabled = false;

        yield return new WaitForSeconds(0.5f);

        if (curve == null)
        {
            curve = GetComponentInChildren<BezierCurve>();
        }

        float progress = 0;
        cam.motionBlure.enabled = blur;

        Vector3 aim = points[1].position;
        //Debug.Log("AIM: " + aim);
        //aim.y = currentCharacterPos.y;
        characterRotation = movement.transform.rotation;
        Quaternion destinationRotation = Quaternion.LookRotation(Vector.Direction(movement.transform.position, points[1].position));
        //float camProgress = 0;
        float smoothProgress = 0;

        lerpFactor = 0;
        while (lerpFactor < 1)
        {
            lerpFactor += 0.1f;
            characterMovement.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpFactor);
            yield return null;
        }

        characterMovement.transform.localScale = Vector3.one;
        poof.transform.position = barrelEnd.position;
        poof.Play(true);
        while (progress < 0.9f)
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
        characterMovement.SetAnimation("JumpDown");
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
        if (shoot && characterMovement)
        {
            characterMovement.transform.position = Vector3.Lerp(characterMovement.transform.position, currentCharacterPos, 0.1f);
            var pos = points[1].position;
            characterMovement.transform.rotation = Quaternion.LookRotation(new Vector3(pos.x, pos.y, pos.z) - characterMovement.transform.position);
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
            data["InsideBarrelRotation"] = (Float4)barrel.rotation;
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
            if (data.ContainsKey("InsideBarrelRotation"))
                barrel.rotation = (Float4)data["InsideBarrelRotation"];
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
        if (points[1])
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(points[1].position, 1);
        }
    }

    public void Activate()
    {
        var character = Character.GetLocalPlayer();
        if(Vector3.Distance(character.transform.position, transform.position) < radius)
        {
            if (!Used)
            {
                StartCoroutine(JumpToPipeCor(character.movement));
            }
        }
    }
}