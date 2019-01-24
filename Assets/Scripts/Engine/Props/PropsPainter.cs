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
        public Vector3 camPos;


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

        [HideInInspector]
        public Vector3 normal;

        public GameObject target;

        Vector3 prevPos;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(camPos, 0.75f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(camPos, 0.25f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(camPos, camPos + normal * 5);
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

                prop.localScale = randomScale ?  Vector3.one *  Random.Range(minScale, maxScale) : prefabScale;
                prop.SetParent(transform);
                prop.gameObject.AddComponent<Prop>();
                createdProps.Add(prop.gameObject);
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