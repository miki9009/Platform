using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(MeshRendererManager))]
public class MeshRendererEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MeshRendererManager script = (MeshRendererManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Set Renderers for " + script.meshTypeToSet.ToString()))
        {
            SetRenderers(script);
        }
    }

    void SetRenderers(MeshRendererManager manager)
    {
        MeshRenderer[] renderers = null;
        if (manager.meshTypeToSet == MeshRendererManager.MeshType.Static)
        {
         renderers = GameObject.FindObjectsOfType<MeshRenderer>().Where(x => x.gameObject.isStatic).ToArray();
        }
        else
        {
            renderers = GameObject.FindObjectsOfType<MeshRenderer>().Where(x => !x.gameObject.isStatic).ToArray();
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].allowOcclusionWhenDynamic = manager.dynamicOcluded;
            renderers[i].reflectionProbeUsage = manager.reflectionProbeUsage;
            renderers[i].shadowCastingMode = manager.castShadow;
            renderers[i].receiveShadows = manager.recieveShadows;
            renderers[i].motionVectorGenerationMode = manager.motionVectors;
            if (manager.removeAnimationController)
            {
                var anim = renderers[i].GetComponent<Animator>();
                if (anim != null)
                {
                    DestroyImmediate(anim);
                }
            }
        }
    }

}
