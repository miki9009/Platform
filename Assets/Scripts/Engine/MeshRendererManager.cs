using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererManager : MonoBehaviour
{
    public enum MeshType { Static, Realtime}

    public MeshType meshTypeToSet;
    public ReflectionProbeUsage reflectionProbeUsage;
    public ShadowCastingMode castShadow;
    public bool recieveShadows;
    public MotionVectorGenerationMode motionVectors;
    public bool optimizeRealtimeUVs;
    public bool ignoreNormals;
    public bool dynamicOcluded;
    public bool removeAnimationController;
}
