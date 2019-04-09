using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Engine.Config
{ 
    [CreateAssetMenu(menuName = key)]
    public class QualityConfig : Config
    {


        public const string key = "Configs/QualityConfig";

        public List<QualityMode> qualityModes;
    }
        [Serializable]
        public class QualityMode
        {
            public GameQualitySettings.GameQuality qualityType;
            [Range(0.2f, 1f)]
            public float resolutionRange;
            [Range(0, 6)]
            public int UnityQualitySettings = 0;
            [Range(50, 500)]
            public int cameraFarClip = 100;
            public List<MaterialReplacement> materialReplacements;
        }

        [Serializable]
        public class MaterialReplacement
        {
            public Material material;
            public Shader shader;
        }
    
}
