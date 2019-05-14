using Engine;
using UnityEngine;

public class StoneHeadFlameThrower : LevelElement
{
    public float timeBetweenFire = 1;
    public float fireDuration = 1;

    public ParticleSystem fireParts;
    public ParticleSystem smokeParts;

    float curTime;
    float curFireDuration;

    private void Update()
    {
        if (curTime < timeBetweenFire)
            curTime += Time.deltaTime;
        else
        {
            if (!fireParts.isPlaying)
                fireParts.Play(true);
            if (curFireDuration <= fireDuration)
                curFireDuration += Time.deltaTime;
            else
            {
                curFireDuration = 0;
                curTime = 0;
                fireParts.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    void OnParticleCollision(GameObject other)
    {
        var character = other.GetComponent<Character>();

        if (character)
        {
            character.movement.Hit(null, 1, false);
        }
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