using Engine;
using System.Collections.Generic;
using UnityEngine;

public class CannonFour : Cannon
{
    public float waitTime = 1;
    int prevRot = 0;
    int rot = 1;
    float curWaitTime;
    float rotationTime;
    public Transform[] barrels;
    bool canShoot;
    float[] rotations = new float[]
    {
        180,
        270,
        0,
        90
    };

    protected override void Update()
    {
        if (triggered)
            if (curWaitTime < waitTime)
            {
                curWaitTime += Time.deltaTime;
            }
            else
            {
                if (rotationTime < 1)
                {

                    rotationTime += Time.deltaTime;
                    cannon.localRotation = Quaternion.Lerp(cannon.localRotation, Quaternion.Euler(0, rotations[rot], 0), rotationTime);
                }
                else
                {
                    curWaitTime = 0;
                    rotationTime = 0;
                    if (rot + 1 < rotations.Length)
                    {
                        rot++;
                    }
                    else
                    {
                        rot = 0;
                    }
                    if (prevRot + 1 < rotations.Length)
                    {
                        prevRot++;
                    }
                    else
                    {
                        prevRot = 0;
                    }
                    barrel = barrels[rot];
                    Shooting();
                }
            }
    }

    void Shooting()
    {
        if (triggered && characters.Count > 0)
        {
            if (target == null)
            {
                float dis = Mathf.Infinity;

                for (int i = 0; i < characters.Count; i++)
                {
                    float dis2 = Vector3.Distance(transform.position, characters[i].transform.position);
                    if (dis2 < dis)
                    {
                        target = characters[i].transform;
                        dis = dis2;
                    }
                }
            }
            else
            {
                    Shoot(target.position,false);
            }

        }
    }
}