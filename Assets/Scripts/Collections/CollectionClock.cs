using UnityEngine;

public class CollectionClock : CollectionObject
{
    [Header("Clock")]
    public int time = 10;
    protected override void Start()
    {
        base.Start();
        OnCollected += AddTime;
    }

    void AddTime(object obj)
    {
        GameTime.Instance.AddTime(time);
    }
}
