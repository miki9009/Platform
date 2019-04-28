using UnityEngine;

namespace Engine
{
    public class SceneLight : MonoBehaviour
    {
        static Light _currentLight;
        public static Light CurrentLight
        {
            get
            {
#if UNITY_EDITOR
                var lights = FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type == LightType.Directional)
                        _currentLight = light;
                }
#endif
                return _currentLight;               
            }
        }

        void Awake()
        {
            _currentLight = GetComponent<Light>();
        }

        private void OnEnable()
        {
            _currentLight = GetComponent<Light>();
        }

        private void OnDestroy()
        {
            _currentLight = null;
        }
    }
}
