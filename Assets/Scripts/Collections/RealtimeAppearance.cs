using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeAppearance : LevelElement
{
    public bool activated;
    public List<ActiveObject> objects;
    public BoxCollider trigger;

    public override void ElementStart()
    {
        base.ElementStart();
        objects = GetAllChildren();
        DetachChildren();
        if (!activated)
        {
            StartCoroutine(DeactivateChildren(5));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activated)
        {
            StartCoroutine(ActivateChildren());
        }
        activated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (activated)
        {
            StartCoroutine(DeactivateChildren(0));
            activated = false;
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        data["TriggerSize"] = (Float3)trigger.size;
        data["TriggerCenter"] = (Float3)trigger.center;
        data["Activated"] = activated;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("TriggerSize"))
        {
            trigger.size = (Float3)data["TriggerSize"];
        }
        if (data.ContainsKey("TriggerCenter"))
        {
            trigger.center = (Float3)data["TriggerCenter"];
        }
        if (data.ContainsKey("Activated"))
        {
            activated = (bool)data["Activated"];
        }
    }



    private List<ActiveObject> GetAllChildren()
    {
        var list = new List<ActiveObject>();
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            var child = transform.GetChild(i);
            var active = child.gameObject.AddComponent<ActiveObject>();
            active.Manager = this;
             list.Add(active);
        }
        return list;
    }

    private void DetachChildren()
    {
        transform.DetachChildren();
    }

    private IEnumerator DeactivateChildren(int waitFrames)
    {
        var wait = new WaitForSeconds(0.1f);
        for (int i = 0; i < waitFrames; i++)
        {
            yield return null;
        }
        var scaleFactor = Vector3.one * 0.1f;
        var scale = Vector3.one;
        int j = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj.gameObject != null && obj.gameObject.activeInHierarchy)
            {
                obj.DeactivatedByManager = true;
                obj.gameObject.SetActive(false);
            }
            j++;
            if (j == 5)
            {
                j = 0;
                yield return null;
            }
        }
        yield return null;
    }

    private IEnumerator ActivateChildren()
    {
        var wait = new WaitForSeconds(0.1f);

        var scaleFactor = Vector3.one * 0.1f;
        var scale = Vector3.one;
        int j = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj.gameObject != null && !obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.transform.localScale = obj.scale;
                obj.gameObject.SetActive(true);
            }
            j++;
            if (j == 5)
            {
                j = 0;
                yield return wait;
            }
        }
        yield return null;
    }
#if UNITY_EDITOR
    public bool drawGizmo;
    Color col = new Color(0, 0, 1, 0.5f);
    private void OnDrawGizmos()
    {
        if(drawGizmo)
        {
            Gizmos.color = col;
            Gizmos.DrawCube(transform.localPosition + trigger.center, trigger.size);
        }

    }

#endif
}
