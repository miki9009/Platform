using System.Collections.Generic;
using UnityEngine;

public class TargetPointerManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    public int spawnsCount = 10;

    static Stack<GameObject> arrows = new Stack<GameObject>();
    static List<TargetPointer> instantiatedArrows = new List<TargetPointer>();

    static TargetPointerManager instance;
    private void Awake()
    {
        instance = this;
        transform.position = Vector3.zero;
        GameManager.LevelClear += PoolAllArrows;
        PrepareSpawns();
    }

    public static TargetPointer PrepareArrow(Transform origin, Transform target)
    {
        var levelElement = target.GetComponent<Engine.LevelElement>();
        if (levelElement == null) return null;
        levelElement.ArrowActivator = levelElement.gameObject.AddComponent<TargetPointerActivator>();
        GameObject arrow = null;
        if(arrows.Count > 0)
        {
            arrow = arrows.Pop();
        }
        else
        {
            arrow = Instantiate(instance.arrowPrefab, instance.transform);
        }

        var targetPointer = arrow.GetComponent<TargetPointer>();
        targetPointer.target = target;
        targetPointer.origin = origin;
        levelElement.ArrowActivator.AssignArrow(targetPointer);
        if (arrow != null)
            arrow.SetActive(true);
        //Debug.Log("Arrow set");
        return targetPointer;
    }

    static void PoolAllArrows()
    {
        Debug.Log("Pooled arrows");
        foreach (var arrow in instantiatedArrows)
        {
            PoolArrow(arrow);
        }
    }

    public static void PoolArrow(TargetPointer arrow)
    {
        if (arrow == null || arrow.gameObject == null) return;
        arrow.target = null;
        arrow.gameObject.SetActive(false);
        arrows.Push(arrow.gameObject);
    }

    void PrepareSpawns()
    {
        for (int i = 0; i < spawnsCount; i++)
        {
            var arrow = Instantiate(arrowPrefab, transform);
            arrow.SetActive(false);
            arrows.Push(arrow);
            instantiatedArrows.Add(arrow.GetComponent<TargetPointer>());
        }
    }


}