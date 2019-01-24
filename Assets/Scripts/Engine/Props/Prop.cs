using UnityEngine;

namespace Engine.Props
{
    [ExecuteInEditMode]
    public class Prop : MonoBehaviour
    {
        public Collider col;
        private void Awake()
        {
            if(!Application.isPlaying)
            {
                if (!GetComponent<Collider>())
                {
                    var sphere = gameObject.AddComponent<SphereCollider>();
                    sphere.radius = 1;
                    col = sphere;
                }
            }
            else if(col)
            {
                Destroy(col);
            }


        }
    }
}
