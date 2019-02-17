using LPWAsset;
using System.Collections.Generic;
using UnityEngine;

public class FloatingManager : MonoBehaviour
{
    public LowPolyWaterScript lowPoly;
    public float lerpSpeed = 0.75f;
    public float waveRange = 5;

    static List<FloatingManager> floatingManagers = new List<FloatingManager>();

    public static FloatingManager GetFloatingManager(Vector3 pos)
    {
        float dis = Mathf.Infinity;
        FloatingManager manager = null;
        for (int i = 0; i < floatingManagers.Count; i++)
        {
            float dis2 = Vector3.Distance(floatingManagers[i].transform.position, pos);
            if(dis2 < dis)
            {
                manager = floatingManagers[i];
                dis = dis2;
            }
        }
        return manager;
    }

    List<FloatingObject> floatingObjects = new List<FloatingObject>();

    float xSize;

    private void Awake()
    {
        Debug.Log("Adding FloatingManager");
        floatingManagers.Add(this);
        xSize = lowPoly.sizeX;
    }

    private void OnDestroy()
    {
        floatingManagers.Remove(this);
    }

    private void Start()
    {
        ApplyStartWaveFactor();
    }

    public void AddObject(FloatingObject floatingObject)
    {
        if (!floatingObjects.Contains(floatingObject))
        {
            floatingObjects.Add(floatingObject);
            floatingObject.Progress = GetStartProgress(floatingObject.transform.position.x);
        }


    }

    public void RemoveObject(FloatingObject floatingObject)
    {
        if (floatingObjects.Contains(floatingObject))
            floatingObjects.Remove(floatingObject);
    }

    private void Update()
    {
        for (int i = 0; i < floatingObjects.Count; i++)
        {
            floatingObjects[i].UpdatePosition(Time.deltaTime, lerpSpeed);
        }
    }

    void ApplyStartWaveFactor()
    {
        for (int i = 0; i < floatingObjects.Count; i++)
        {
            floatingObjects[i].Progress = GetStartProgress(floatingObjects[i].transform.position.x);
        }
    }

    float GetStartProgress(float x)
    {
        int factor = (int)(x / waveRange);
        float newX = factor * waveRange;
        float delta = x - newX;
        return delta / waveRange;
    }
}
