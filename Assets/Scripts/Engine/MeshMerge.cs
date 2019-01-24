using UnityEngine;
using System.Collections;
using System;

namespace Engine
{
    public class MeshMerge : MonoBehaviour
    {
        public MeshFilter[] meshFilters;
        public Transform parent;
        Vector3 pos;
        public bool affectParent = true;

        public GameObject Merge()
        {
            if(parent != null)
            {
                meshFilters = parent.GetComponentsInChildren<MeshFilter>();
            }
            if (meshFilters == null || meshFilters.Length == 0)
            {
                Debug.Log("No meshes");
                return null;
            }
            var parentParent = parent.parent;
            var prevPos = parent.position;
            parent.position = Vector3.zero;
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            pos = Vector3.zero;

            //Exclude Parent
            if(!affectParent)
            {
                if (meshFilters[0].transform == parent.transform)
                {
                    meshFilters[0] = meshFilters[meshFilters.Length - 1];
                    Array.Resize(ref meshFilters, meshFilters.Length - 1);
                }
            }


            while (i < meshFilters.Length)
            {
                pos += meshFilters[i].transform.position;
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].mesh.uv = meshFilters[i].sharedMesh.uv;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            var obj = new GameObject("Combined_"+meshFilters[0].name);
            obj.transform.position = pos/meshFilters.Length;

            var meshRenderer = obj.AddComponent<MeshRenderer>();
            var filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };
            filter.sharedMesh.CombineMeshes(combine);
//#if UNITY_EDITOR
//            UnityEditor.Unwrapping.GenerateSecondaryUVSet(filter.sharedMesh);
//#endif
            meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
            parent.position = prevPos;
            meshRenderer.transform.position = prevPos;
            if(parentParent != null)
            {
                meshRenderer.transform.SetParent(parentParent);
                meshRenderer.transform.SetSiblingIndex(parent.GetSiblingIndex());
            }

            if(!affectParent)
                parent.gameObject.SetActive(false);
            return obj;
        }
    }
}