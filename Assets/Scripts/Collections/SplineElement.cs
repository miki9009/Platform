using UnityEngine;
using Engine;

public class SplineElement : LevelElement
{
    
    public BezierCurve curve;
    public float speed;
    public Transform[] points;
    [SpawnsSelector]
    public string[] spawns;
    public bool moving;

    public bool cached;
    public int cachedPoints;


    Vector3 prevPos;
    float calculatedSpeed;
    float calculatedRotationSpeed;
    Vector3 lastPos;
    float dis;
    Vector3 dir;
    Transform[] elements;
    Engine.Threads.Host host;
    private void Awake()
    {
        curve = GetComponentInChildren<BezierCurve>();
    }

    Float3[] pointsPos;
    Float4[] pointsRot;
    Float3[] handle1Pos;
    Float3[] handle2Pos;

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
            data["Spawns"] = spawns;
            data["Moving"] = moving;
            data["Speed"] = speed;
            data["Cached"] = cached;
            data["CachedPoints"] = cachedPoints;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            if (data.ContainsKey("Points"))
                pointsPos = (Float3[])data["Points"];
            if (data.ContainsKey("Rotations"))
                pointsRot = (Float4[])data["Rotations"];
            if (data.ContainsKey("Handle1"))
                handle1Pos = (Float3[])data["Handle1"];
            if (data.ContainsKey("Handle2"))
                handle2Pos = (Float3[])data["Handle2"];
            if (data.ContainsKey("Spawns"))
                spawns = (string[])data["Spawns"];
            if (data.ContainsKey("Moving"))
                moving = (bool)data["Moving"];
            if (data.ContainsKey("Speed"))
                speed = (float)data["Speed"];
            if (data.ContainsKey("Cached"))
                cached = (bool)data["Cached"];
            if (data.ContainsKey("CachedPoints"))
                cachedPoints = (int)data["CachedPoints"];
            for (int i = 0; i < pointsPos.Length; i++)
            {
                points[i].transform.localPosition = pointsPos[i];
                points[i].transform.localRotation = pointsRot[i];
                var handle = points[i].GetComponent<BezierPoint>();
                handle.handle1 = handle1Pos[i];
                handle.handle2 = handle2Pos[i];
            }

            if(Application.isPlaying)
            {
                splineFactors = new float[spawns.Length];
                elements = new Transform[spawns.Length];
                positions = new Vector3[spawns.Length];
                float splineFactor = 1f / spawns.Length;
                for (int i = 0; i < spawns.Length; i++)
                {
                    var spawn = (GameObject)Resources.Load(PrefabsPath + spawns[i], typeof(GameObject));
                    if (spawn != null)
                    {
                        elements[i] = Instantiate(spawn, transform).GetComponent<Transform>();
                        splineFactors[i] = i * splineFactor;
                        elements[i].transform.position = curve.GetPointAt(splineFactors[i]);
                    }
                    else
                    {
                        Debug.LogError("Spawn was null: " + spawns[i]);
                    }
                }

                if(cached)
                {
                    InitCache();
                }
            }
        }
    }
    Vector3 pos;
    Vector3[] cachedPos;
    float[] splineFactors;
    float curFactor;
    float point;
    Vector3[] positions;
    int[] indexes;

    private void Update()
    {
        if (moving)
        {
            if(cached)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if(Vector3.Distance(elements[i].position, cachedPos[indexes[i]]) < 1)
                    {
                        positions[i] = GetNextPos(ref indexes[i]);                       
                    }
                    elements[i].position = Vector3.MoveTowards(elements[i].position, positions[i], Time.deltaTime * speed);
                }
            }
            else
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    point = (curFactor + splineFactors[i]) % 1;
                    elements[i].position = curve.GetPointAt(point);
                }
                curFactor += speed * Time.deltaTime;
                if (curFactor >= 1)
                    curFactor = curFactor % 1;
            }
        }
    }


/// ////////////////////CACHED//////////////////////////////////////
    void InitCache()
    {
        float factor = 1f / (float)cachedPoints;
        cachedPos = new Vector3[cachedPoints];
        for (int i = 0; i < cachedPoints; i++)
        {
            cachedPos[i] = curve.GetPointAt(factor * i);
        }

        indexes = new int[spawns.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            indexes[i] = GetNearest(elements[i].position);
            positions[i] = elements[i].position;
        }

    }

    int GetNearest(Vector3 pos)
    {
        var dis = Mathf.Infinity;
        float dis2;
        int nearest = 0;
        for (int i = 0; i < cachedPos.Length; i++)
        {
            dis2 = Vector3.Distance(pos, cachedPos[i]);
            if(dis > dis2)
            {
                dis = dis2;
                nearest = i;
            }
        }
        return nearest;
    }

    Vector3 GetNextPos(ref int indexValue)
    {
        if (indexValue < cachedPos.Length - 1)
            indexValue++;
        else
            indexValue = 0;
        return cachedPos[indexValue];
    }

/// ////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (curve == null) return;
        for (int i = 0; i < spawns.Length; i++)
        {
            float factor = 1f / spawns.Length;
            Gizmos.DrawSphere(curve.GetPointAt(factor * i), 1);
        }
    }
#endif

}
