using LPWAsset;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    protected FloatingManager manager;

    public float upFactor = 0.5f;
    public float downFactor = 0.5f;

    protected Vector3 upPos;
    protected Vector3 downPos;
    private void Start()
    {
        upPos = transform.localPosition + Vector3.up * upFactor;
        downPos = transform.localPosition + Vector3.down * downFactor;
        if(!manager)
        {
            OnEnable();
        }
    }

    private void OnEnable()
    {
        manager = FloatingManager.GetFloatingManager(transform.position);
        if (manager)
        {
            manager.AddObject(this);
        }

    }

    private void OnDisable()
    {
        if (manager)
            manager.RemoveObject(this);
    }

    private void OnDestroy()
    {
        if (manager)
            manager.RemoveObject(this);
    }

    public bool up;
    public float Progress
    {
        get
        {
            return progress;
        }
        set
        {
            if (value > 0)
                up = true;
            else
                up = false;
            progress = Mathf.Abs(value);
        }
    }
    protected float progress = 0;
    public virtual void UpdatePosition(float deltaTime, float lerpSpeed)
    {
        if (up)
        {
            progress += deltaTime * lerpSpeed;
            if (progress > 1)
            {
                progress = 1f;
                up = false;
            }
        }
        else
        {
            progress -= deltaTime * lerpSpeed;
            if (progress < 0)
            {
                progress = 0;
                up = true;
            }
        }

        var pos = transform.localPosition;
        var posNew = Vector3.Lerp(downPos, upPos, progress);
        pos.y = posNew.y;
        transform.localPosition = pos;
    }
}