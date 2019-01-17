using System;
using System.Collections;
using UnityEngine;

public class PowerUpGrowth : PowerUp
{
    public CollectionObject CollectionObject { get; set; }
    public float bigScaleFactor = 2;
    public float scaleSpeedFactor = 2;
    public float timeActive = 15;
    public GameObject partsPrefab;
    ParticleSystem parts;
    Vector3 startScale;

    public void BackToCollection()
    {

    }

    protected override void Start()
    {
        base.Start();
        col = GetComponent<SphereCollider>();
        GetComponent<CollectionObject>().OnCollected += (GameObject obj) =>
        {
            col.enabled = false;
            character = obj.GetComponentInParent<Character>();
            ApplyPowerUp();
        };
        GameManager.LevelClear += ClearCoroutine;
        parts= Instantiate(partsPrefab, transform.position, Quaternion.identity).GetComponentInChildren<ParticleSystem>();
    }

    private void ClearCoroutine()
    {
        if (coroutine != null)
            CoroutineHost.Stop(Scale());
        if (Controller.Instance != null && Controller.Instance.gameCamera != null)
            Controller.Instance.gameCamera.motionBlure.enabled = false;
    }

    Coroutine coroutine;
    protected override void ApplyPowerUp()
    {
        base.ApplyPowerUp();
        character.movement.Invincible = true;
        startScale = character.transform.localScale;
        parts.Play();
        coroutine = CoroutineHost.Start(Scale());
    }

    public override void Disable()
    {
        base.Disable();
        character.movement.Invincible = false;
    }

    public void Remove()
    {
        Disable();
    }

    IEnumerator Scale()
    {
        float time = 0;
        Controller.Instance.gameCamera.motionBlure.enabled = true;
        while (time < 1)
        {
            time += Time.deltaTime * scaleSpeedFactor;
            character.transform.localScale = Vector3.Lerp(character.transform.localScale, startScale * 2, time);
            yield return null;
        }
        Controller.Instance.gameCamera.motionBlure.enabled = false;
        while (timeActive > 0)
        {
            timeActive -= Time.deltaTime;
            yield return null;
        }
        Controller.Instance.gameCamera.motionBlure.enabled = true;
        time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * scaleSpeedFactor;
            if (character == null) yield break;
            character.transform.localScale = Vector3.Lerp(character.transform.localScale, startScale, time);
            yield return null;
        }
        Controller.Instance.gameCamera.motionBlure.enabled = false;
        coroutine = null;
        Disable();
    }
}