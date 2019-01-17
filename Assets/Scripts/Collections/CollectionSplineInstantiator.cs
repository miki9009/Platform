using Engine;
using System.Collections;
using UnityEngine;

public class CollectionSplineInstantiator : LevelElement
{
    public BezierCurve curve;
    public GameObject mainCoin;
    public Transform[] points;
    public int amount;
    [SpawnsSelector]
    public string spawnName;
    public float animationSpeed = 2;

    public AnimationCurve animationCurve;

    Transform[] elements;
    Float3[] pointsPos;
    Float4[] pointsRot;
    Float3[] handle1Pos;
    Float3[] handle2Pos;
    public float yFactor = 2;

    bool triggered = false;

    public override void ElementStart()
    {
        base.ElementStart();
        var spawn = (GameObject)Resources.Load(PrefabsPath + spawnName, typeof(GameObject));
        if (spawn != null)
        {
            float splineFactor = 1f / amount;
            elements = new Transform[amount];
            for (int i = 0; i < amount; i++)
            {
                elements[i] = Instantiate(spawn, transform).GetComponent<Transform>();
                elements[i].transform.position = curve.GetPointAt(i * splineFactor);
                elements[i].gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Spawn was null: " + spawn);
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        int length = points.Length;
        pointsPos = new Float3[length];
        pointsRot = new Float4[length];
        handle1Pos = new Float3[length];
        handle2Pos = new Float3[length];
        for (int i = 0; i < length; i++)
        {
            pointsPos[i] = points[i].localPosition;
            pointsRot[i] = points[i].localRotation;
            var handle = points[i].GetComponent<BezierPoint>();
            handle1Pos[i] = handle.handle1;
            handle2Pos[i] = handle.handle2;
        }
        if (data != null)
        {
            data["Points"] = pointsPos;
            data["Rotations"] = pointsRot;
            data["Handle1"] = handle1Pos;
            data["Handle2"] = handle2Pos;
            data["Spawn"] = spawnName;
            data["Amount"] = amount;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("Points"))
            pointsPos = (Float3[])data["Points"];
        if (data.ContainsKey("Rotations"))
            pointsRot = (Float4[])data["Rotations"];
        if (data.ContainsKey("Handle1"))
            handle1Pos = (Float3[])data["Handle1"];
        if (data.ContainsKey("Handle2"))
            handle2Pos = (Float3[])data["Handle2"];
        if (data.ContainsKey("Spawn"))
            spawnName = (string)data["Spawn"];
        if (data.ContainsKey("Amount"))
            amount = (int)data["Amount"];

        for (int i = 0; i < pointsPos.Length; i++)
        {
            points[i].transform.localPosition = pointsPos[i];
            points[i].transform.localRotation = pointsRot[i];
            var handle = points[i].GetComponent<BezierPoint>();
            handle.handle1 = handle1Pos[i];
            handle.handle2 = handle2Pos[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered)
        {
            triggered = true;
            mainCoin.SetActive(false);
            enabled = false;
            StartCoroutine(StartAnimation());
        }

    }

    private void Update()
    {
        mainCoin.transform.rotation = CollectionObject.rotation;
    }

    IEnumerator StartAnimation()
    {
        float progress = 0;
        int i = 0;
        bool[] triggered = new bool[amount];
        float factor = 1f / amount;
        while (progress < 1)
        {
            progress += animationSpeed * Time.deltaTime;
            if (i > amount - 1)
            {
                continue;
            }
            if(triggered[i] == false && progress > factor * i)
            {
                triggered[i] = true;
                elements[i].gameObject.SetActive(true);
                StartCoroutine(Animation(elements[i]));
                i++;
            }
            yield return null;
        }

    }

    IEnumerator Animation(Transform element)
    {
        float animation = 0;
        Vector3 startPos = element.position;
        element.gameObject.SetActive(true);
        while (animation < 1)
        {
            element.position = new Vector3(startPos.x, startPos.y + yFactor * animationCurve.Evaluate(animation), startPos.z);
            animation += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (curve == null) return;
        for (int i = 0; i < amount; i++)
        {
            float factor = 1f / amount;
            Gizmos.DrawSphere(curve.GetPointAt(factor * i), 1);
        }
    }
#endif
}