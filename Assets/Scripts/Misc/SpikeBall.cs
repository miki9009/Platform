using Engine;
using UnityEngine;

public class SpikeBall : LevelElement
{
    public Transform spikeball;
    public float speed;
    public AnimationCurve curve;
    public TriggerBroadcast triggerBroadcast;

    bool forward;
    float time;


    private void Awake()
    {
        triggerBroadcast.TriggerEntered += TriggerBroadcast_TriggerEntered;
    }
    private void OnDestroy()
    {
        triggerBroadcast.TriggerEntered -= TriggerBroadcast_TriggerEntered;
    }

    private void TriggerBroadcast_TriggerEntered(Collider col)
    {
        if(col.gameObject.layer == Layers.Character)
        {
            var character = col.gameObject.GetComponent<Character>();
            if(character)
            {
                character.movement.Hit(null, 1000, true);
            }
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("Speed"))
            speed = (float)data["Speed"];
        if (data.ContainsKey("KeyFrames"))
            curve.keys = Engine.Keyframe.ToArray((Engine.Keyframe[])data["KeyFrames"]);
    }

    public override void OnSave()
    {
        base.OnSave();
        data["Speed"] = (float)speed;
        data["KeyFrames"] = Engine.Keyframe.ToArray(curve.keys);

    }

    float curveEvaluated;
    private void Update()
    {
        if(forward)
        {
            time += speed * Time.deltaTime;
        }
        else
        {
            time -= speed * Time.deltaTime;
        }
        if (Mathf.Abs(time ) >= 1)
        {
            forward = !forward;
        }
        curveEvaluated = curve.Evaluate(time);


        transform.eulerAngles = new Vector3(curveEvaluated * 90, transform.eulerAngles.y, transform.eulerAngles.z);
    }


}