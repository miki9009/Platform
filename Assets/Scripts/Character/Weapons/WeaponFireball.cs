﻿using Engine;
using System.Collections;
using UnityEngine;

public class WeaponFireball : Weapon
{
    public float coolTime = 2;
    bool canShoot = true;
    public Vector3 defaultForearmPos = new Vector3(-12.7f, 64.25f, 21.25f);
    public Vector3 defaultUpperarmPos = new Vector3(-44.3f, -22.7f, -95.2f);
    public Vector3 maxAngle = new Vector3(-66.16f, -119.2f, -0.8f);
    public Vector3 minAngle = new Vector3(51.48f, 3.8f, -87f);
    public float animationSpeed = 4;

    [HideInInspector]
    public Transform lowerArm;
    [HideInInspector]
    public Transform upperArm;

    public override void Attack()
    {
        if (!canShoot) return;
        progress = 0;
        defaultPos = false;
        canShoot = false;

        ShootProjectile();
    }

    public override void Apply()
    {
        base.Apply();
        if(character)
        {
            character.movement.MeleeAttack = Attack;
            character.movement.StopAttack = StopAttack;
            lowerArm = character.rightLowerArm;
            upperArm = character.rightUpperArm;
        }
        else
        {
            enabled = false;
            Debug.LogError("Character component not found on object and parents");
        }
    }

    void SetShooting()
    {
        canShoot = true;
    }

    bool defaultPos = true;
    float progress;
    int stage = 0;
    private void LateUpdate()
    {
        //lowerArm.localEulerAngles = defaultForearmPos;

        //if (defaultPos)
        //{
        //    if (progress > 0)
        //    {
        //        upperArm.localEulerAngles = Vector3.Lerp(defaultUpperarmPos, minAngle, progress);
        //        progress -= Time.deltaTime * animationSpeed;
        //    }
        //    else
        //        upperArm.localEulerAngles = defaultUpperarmPos;
        //}
        //else
        //{
        //    if(progress < 1)
        //    {
        //        if(stage == 0)
        //        {
        //            upperArm.localEulerAngles = Vector3.Lerp(defaultUpperarmPos, maxAngle, progress);
        //            progress += Time.deltaTime * animationSpeed * 2;
        //            if(progress >= 1)
        //            {
        //                stage = 1;
        //                progress = 0;
        //                ShootProjectile();
        //            }
        //        }
        //        else
        //        {
        //            upperArm.localEulerAngles = Vector3.Lerp(maxAngle, minAngle, progress);
        //            progress += Time.deltaTime * animationSpeed;
        //        }
        //    }
        //    else
        //    {
        //        upperArm.localEulerAngles = Vector3.Lerp(maxAngle, minAngle, progress);
        //        defaultPos = true;
        //        stage = 0;
        //        progress = 1;
        //    }
        //}
    }

    void ShootProjectile()
    {
        var spawn = SpawnManager.GetSpawn("Fireball", true);
        var fireball = spawn.GetComponent<Fireball>();
        var pos = character.transform.position;
        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit[] hits = Physics.SphereCastAll(pos, attackRadius * 3, Vector3.down, 10, character.movement.collisionLayer.value, QueryTriggerInteraction.Ignore);
        float dot = -100;
        float dot2;
        Transform enemy = null;
        foreach (var hit in hits)
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform == character.transform) continue;
            //angle2 = Vector3.Angle(character.transform.position, hit.point);
            dot2 = Vector3.Dot(character.transform.forward, Engine.Vector.Direction(character.transform.position, hit.transform.position)) * 100;
            if (dot2 > 0)
            {
                dot = dot2;
                var enemyComponent = hit.transform.GetComponent<IDestructible>();
                if (enemyComponent != null)
                {
                    if (enemyComponent.Destroyed) continue;

                    if (enemy)
                    {
                        if (Vector3.Distance(hit.transform.position, character.transform.position) < Vector3.Distance(enemy.position, character.transform.position))
                            enemy = hit.transform;
                    }
                    else
                    {
                        enemy = hit.transform;
                    }
                }
                else
                {
                    enemyComponent = hit.transform.GetComponentInParent<IDestructible>();
                    if (enemyComponent != null)
                    {
                        if (enemyComponent.Destroyed) continue;
                        if (enemy)
                        {
                            if (Vector3.Distance(hit.transform.position, character.transform.position) < Vector3.Distance(enemy.position, character.transform.position))
                                enemy = enemyComponent.Transform;
                        }
                        else
                        {
                            enemy = enemyComponent.Transform;
                        }
                    }
                }
            }
        }
        if (enemy)
            character.transform.rotation = Quaternion.LookRotation(Vector.Direction(character.transform.position, enemy.transform.position));

        fireball.Shoot(character.rightLowerArm.position, character.transform.forward, character);
        Invoke("SetShooting", coolTime);
    }




}

