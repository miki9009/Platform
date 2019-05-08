using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Engine;


public class Teleport : LevelElement
{
    public Teleport otherTeleport;
    public GameObject teleportPrefab;
    Transform character;
    public bool canTeleport = true;

    Float3 otherTeleportPosition;
    bool originalTeleport;

    private void OnTriggerEnter(Collider other)
    {
        if (canTeleport && other.gameObject.layer == Layers.Character && otherTeleport.canTeleport)
        {
            canTeleport = false;
            otherTeleport.canTeleport = false;
            character = other.transform.root;
            StartCoroutine(Teleportation());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!canTeleport && other.gameObject.layer == Layers.Character)
        canTeleport = true;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (Application.isPlaying && data.ContainsKey("OtherTeleportPosition"))
        {
            otherTeleportPosition = (Float3)data["OtherTeleportPosition"];
            if (!otherTeleport)
                otherTeleport = Instantiate(teleportPrefab).GetComponent<Teleport>();
            otherTeleport.transform.position = otherTeleportPosition;
            otherTeleport.otherTeleport = this;         
        }

    }

    public Transform editorTeleport;
    public override void OnSave()
    {
        base.OnSave();
        if(editorTeleport)
        {
            otherTeleportPosition = editorTeleport.transform.position;
            data["OtherTeleportPosition"] = otherTeleportPosition;
        }
        else
        {
            Debug.LogError("No other teleport");
        }

    }

    IEnumerator Teleportation()
    {
        if (!character) yield break;
        var ch = character.GetComponent<CharacterMovement>();
        if(ch)
        {
            ch.MovementEnabled = false;
        }
        else
        {
            Debug.LogError("CharacterMovement not found on gameobject: " + character.name);
        }
        VignetteAndChromaticAberration visualEffect = Controller.Instance.chromaticAberration;
        Vortex vortex = Controller.Instance.vortex;
        visualEffect.enabled = true;
        vortex.enabled = true;
        float intensity = 0.7f;
        float angle = 0;
        while (angle < 359f)
        {
            angle += 30;
            vortex.angle = angle;
            yield return null;
        }

        while (intensity < 0.98f)
        {
            //intensity += 0.01f;
            intensity += 0.02f;
            visualEffect.intensity = intensity;
            yield return null;
        }
        otherTeleport.canTeleport = false;
        character.position = otherTeleport.transform.position;
        visualEffect.intensity = 1;
        yield return new WaitForSeconds(0.5f);
        while (intensity > 0.7f)
        {
            intensity -= 0.02f;
            visualEffect.intensity = intensity;
            yield return null;
        }
        while (angle > 0f)
        {
            angle -= 30;
            vortex.angle = angle;           
            yield return null;
        }
        visualEffect.enabled = false;
        vortex.enabled = false;
        if(ch)
        {
            ch.MovementEnabled = true;
        }
        yield return null;
    }



#if UNITY_EDITOR


    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if(!editorTeleport)
        {
            editorTeleport = new GameObject("EditorTeleport").transform;
            editorTeleport.SetParent(transform);
            editorTeleport.position = otherTeleportPosition;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(otherTeleportPosition + Vector3.up*1.5f, new Vector3(2, 3, 2));
        Gizmos.DrawLine(transform.position, editorTeleport.position);
        otherTeleportPosition = editorTeleport.position;
    }
#endif
}





