using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CollectionAnimator : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            CollectionObject.eulers = new Vector3(0, CollectionObject.eulers.y + 4, 0);
            CollectionObject.rotation = Quaternion.Euler(CollectionObject.eulers);
            yield return null;
        }
    }
}

