using UnityEngine;
using Engine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Engine.Props
{
    public class PropsPainter : MonoBehaviour
    {
        public bool erase;
        public bool continues = false;
        public float eraseRadius;
        public LayerMask collisionLayer;
        public Transform prefabContainer;



        public GameObject propPrefab;

        public List<GameObject> createdProps;
        [Header("Drawing Tools")]
        public float minDistanceBwetweenProps = 1;
        public bool useNormalRotation;
        public Vector3 prefabRotation;
        public Vector3 prefabScale;
        public bool randomRotation;
        public Vector2 yRot;
        public Vector2 xRot;
        public Vector2 zRot;
        public bool randomScale;
        public float minScale;
        public float maxScale;

        public bool useRandomPrefabs;
        public List<GameObject> randomPrefabs;

        [HideInInspector]
        public Vector3 camPos;

        [HideInInspector]
        public Vector3 normal;

        [HideInInspector]
        public GameObject target;

        Vector3 prevPos;
        Bounds bounds;
        GameObject prevPrefab;
        Vector3 scale;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(camPos, 0.75f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(camPos, 0.25f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(camPos, camPos + normal * 5);
            if(!erase)
            {
                if(prevPrefab != propPrefab)
                {
                    var mesh = propPrefab.GetComponentInChildren<MeshRenderer>();
                    bounds = mesh.bounds;
                    prevPrefab = propPrefab;
                    if(mesh.transform.parent!=null)
                    {
                        scale = mesh.transform.lossyScale;
                    }
                    else
                    {
                        scale = mesh.transform.localScale;
                    }


                }
                Gizmos.color = Color.green;
                var s = new Vector3(scale.x * prefabScale.x, scale.y * prefabScale.y, scale.z * prefabScale.z);
                Gizmos.DrawWireCube(camPos, new Vector3(bounds.size.x * s.x, bounds.size.y * s.y, bounds.size.z * s.z));
            }
        }

        public void CreateProp()
        {
            if(erase)
            {
                RemoveProp();
                return;
            }
            if (!propPrefab) return;

            if(Vector3.Distance(prevPos, camPos) > minDistanceBwetweenProps)
            {
                var targetTransform = target.transform;
                var children = targetTransform.childCount;
                for (int i = 0; i < children; i++)
                {
                    if (Vector3.Distance(camPos, targetTransform.GetChild(i).position) < minDistanceBwetweenProps)
                        return;
                }
                prevPos = camPos;
                var prop = Instantiate(propPrefab, camPos,  Quaternion.Euler(randomRotation ? 
                    new Vector3(Random.Range(xRot.x,xRot.y), Random.Range(yRot.x, yRot.y), Random.Range(zRot.x, zRot.y)): prefabRotation)).transform;

                prop.localScale = prefabScale;
                prefabScale = randomScale ? Vector3.one * Random.Range(minScale, maxScale) : prefabScale;
                prop.SetParent(prefabContainer ? prefabContainer : transform);
                prop.gameObject.AddComponent<Prop>();
                createdProps.Add(prop.gameObject);

                if(useRandomPrefabs)
                {
                    propPrefab = randomPrefabs[Random.Range(0, randomPrefabs.Count)];
                }
            }
        }

        public void RemoveProp()
        {
            var colliders = Physics.OverlapSphere(camPos,eraseRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i] &&  colliders[i].GetComponent<Prop>())
                {
                    DestroyImmediate(colliders[i].gameObject);
                }
            }
        }

        public void CheckEmptyProps()
        {
            for (int i = 0; i < createdProps.Count; i++)
            {
                if (!createdProps[i])
                    createdProps.RemoveAt(i);
            }
        }

    }
}