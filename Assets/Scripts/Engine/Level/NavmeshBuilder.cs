using UnityEngine.AI;
using UnityEngine;

namespace Engine
{
    public class NavmeshBuilder : MonoBehaviour
    {
        public NavMeshSurface surface;

        private void Start()
        {
            surface.BuildNavMesh();
        }
    }
}

