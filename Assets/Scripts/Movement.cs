using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour 
{


    void Update ()
    {
        Move();
	}

    protected abstract void Move();
}
