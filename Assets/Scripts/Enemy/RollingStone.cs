using Engine;
using UnityEngine;

public class RollingStone : LevelElement
{
    public Rigidbody rb;
    public float waitAtStart = 0;

    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RollingStoneTrigge")
            Restart();
    }

    private void Restart()
    {
        transform.position = startPos;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data.ContainsKey("WaitAtStart"))
        {
            waitAtStart = (float)data["WaitAtStart"];
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == Layers.Character)
        {
            var character = collision.gameObject.GetComponent<Character>();
            if (character)
            {
                character.movement.Hit(null, 100, true);
            }
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data["WaitAtStart"] = waitAtStart;
    }

    public override void ElementStart()
    {
        base.ElementStart();
        if (waitAtStart > 0)
        {
            rb.isKinematic = true;
            Invoke("Restart", waitAtStart);
        }
    }
}