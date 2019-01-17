using Engine;
using System.Collections;
using UnityEngine;

public class JumpBox : LevelElement
{
    public SkinnedMeshRenderer skinMesh;
    public float expandSpeed = 2;
    float blendShape = 100;
    bool active = false;
    public Bounds bounds;
    public BoxCollider boxCollider;
    Character character;
    public float force = 100;
    public float upFactor = 2;
    Bounds startBounds;
    bool useUp;
    private void Awake()
    {
        bounds.size = bounds.size / 2;
        startBounds = boxCollider.bounds;
        startBounds.center = new Vector3(0, -1.09f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Layers.Character && ! active)
        {
            character = other.gameObject.GetComponent<Character>();
            StartCoroutine(Expand());
        }

    }

    private void FixedUpdate()
    {
        if(useUp && character!=null)
        {
            character.transform.position += Vector3.up * upFactor;
        }
    }

    bool changedMotionBlur;
    IEnumerator Expand()
    {
        useUp = true;
        active = true;
        float time = 0;
        character.movement.smoke2.gameObject.SetActive(false);
        character.movement.smoke.gameObject.SetActive(false);
        character.rb.velocity = Vector3.zero;
        while (time < 1)
        {
            time += Time.deltaTime * expandSpeed;
            skinMesh.SetBlendShapeWeight(0,blendShape - (time * 100));
            boxCollider.size = Vector3.Lerp(boxCollider.size, bounds.size, time);
            boxCollider.center = Vector3.Lerp(boxCollider.center, bounds.center, time);
            yield return null;
        }
        character.rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
        useUp = false;
        //character.movement.anim.StopPlayback();
        //character.movement.anim.Play("JumpUp");

        StartCoroutine(Shrink());
    }

    IEnumerator Shrink()
    {
        character.movement.smoke2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        character.movement.smoke2.gameObject.SetActive(true);
        character.movement.smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        character.movement.smoke.gameObject.SetActive(true);
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * expandSpeed / 4;
            skinMesh.SetBlendShapeWeight(0, (time * 100));
            boxCollider.size = Vector3.Lerp(boxCollider.size, startBounds.size, time);
            boxCollider.center = Vector3.Lerp(boxCollider.center, startBounds.center, time);
            yield return null;
        }
        active = false;
    }

    public override void OnLoad()
    {
        base.OnLoad();
    }

    public override void OnSave()
    {
        base.OnSave();
    }
}