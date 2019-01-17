using System.Collections;
using UnityEngine;

public class Platfrom : MonoBehaviour
{
    public Transform[] anchors;
    public float speed;
    public bool activated;
    public bool infiniteLoop;
    public int index = 0;


    private IEnumerator Move()
    {
        Vector3.MoveTowards(transform.position, anchors[index].position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, anchors[index].position) < speed)
        {
            if (index < anchors.Length - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
        }
        yield return null;
    }

    public void Activate()
    {
        StartCoroutine(Move());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
    }
}