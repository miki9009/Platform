using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Collections;

public class Turbo : LevelElement
{
    public float activeTime;
    List<Character> characters;
    public float force = 10;
    public ParticleSystem parts;
    float offset = 0;
    public Transform cubeTransform;
    public AnimationCurve curve;

    //public Material mat;
    public MeshRenderer meshRenderer;
    float animationTime = 0;

    private void Awake()
    {
        characters = new List<Character>();
        //meshRenderer.material = mat;
    }


    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();
        if (!character) return;

        if(!characters.Contains(character))
        {
            characters.Add(character);
            StartCoroutine(IncreaseCharacterSpeed(character, activeTime));
        }
    }

    IEnumerator IncreaseCharacterSpeed(Character character, float time)
    {
        float speedMultiplier = 0;
        parts.Play(true);
        var cam = Controller.Instance.gameCamera.mainCamera;
        //Controller.Instance.motionBlur.enabled = true;
        cam.fieldOfView = 45;
        while (time > 0)
        {
            if (cam.fieldOfView < 70)
                cam.fieldOfView++;
            character.movement.rb.AddForce(speedMultiplier * character.transform.forward * force, ForceMode.VelocityChange);       
            time -= Time.deltaTime;
            if (speedMultiplier < 1)
            {
                speedMultiplier += 0.1f;
            }
            parts.transform.position = character.transform.position;
            //parts.transform.rotation = Quaternion.Inverse(character.transform.rotation);
            yield return null;
        }
        //Controller.Instance.motionBlur.enabled = false;
        if (characters.Contains(character))
            characters.Remove(character);
        yield return null;

        while (cam.fieldOfView > 60)
        {
            cam.fieldOfView--;
            yield return null;
        }
        cam.fieldOfView = 60;
        parts.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void Update()
    {
        offset += force * Time.deltaTime;
        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(-offset, 0));

        if (animationTime < 1)
        {
            animationTime += Time.deltaTime;
        }
        else
        {
            animationTime = 0;
        }
        cubeTransform.localScale = new Vector3(3, 1.18f, curve.Evaluate(animationTime));

    }


}