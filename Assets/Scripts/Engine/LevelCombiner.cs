using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Engine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelCombiner : MonoBehaviour
{
    public bool useLevelCombiner;
    public MeshMerge meshCombiner;
    public List<MeshRenderer> meshes;
    public float chunkSizeX = 10;
    public float chunkSizeZ = 10;
    public float maxX = 200;
    public float maxZ = 200;

    public Material[] materials;

    Dictionary<MeshRenderer, MeshFilter> meshCollection;
    Chunk[,] chunks;

    public void Execute()
    {
        GetAllMeshes();
        if(meshes == null || meshes.Count == 0)
        {
            Debug.Log("No meshes selected");
            return;
        }
        materials = GetMaterials();
        CreateCollection();
        CreateChunks();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < longLength; j++)
            {
                Debug.Log("Checking chunk [" + i + ", " + j + "]");
                if (chunks[i,j] != null)
                {
                    Search(chunks[i, j]);
                    CreateMeshChunk(chunks[i, j]);
                    Debug.Log("Chunk [" + i + ", " + j + "] Created succesfully");
                }
                else
                {
                    Debug.Log("Chunk [" + i + ", " + j + "] was null");
                }
            }
        }
    }

    void CreateCollection()
    {
        meshCollection = new Dictionary<MeshRenderer, MeshFilter>();
        for (int i = 0; i < meshes.Count; i++)
        {
            var filter = meshes[i].GetComponent<MeshFilter>();
            meshCollection.Add(meshes[i], filter);
        }
    }

    public void GetAllMeshes()
    {
        meshes = FindObjectsOfType<MeshRenderer>().ToList();
    }

    public List<MeshFilter> GetAllFilters()
    {
        return FindObjectsOfType<MeshFilter>().ToList();
    }

    Material[] GetMaterials()
    {
        Dictionary<int, Material> materialsDictionary = new Dictionary<int, Material>();

        for (int i = 0; i < meshes.Count; i++)
        {
            var mat = meshes[i].sharedMaterial;
            if (!materialsDictionary.ContainsKey(mat.GetHashCode()))
                materialsDictionary.Add(mat.GetHashCode(), mat);
        }

        var mats = new Material[materialsDictionary.Count];
        int index = 0;
        foreach(var mat in materialsDictionary.Values)
        {
            mats[index] = mat;
            index++;
        }
        return mats;
    }
    int length;
    int longLength;
    public void CreateChunks()
    {
        Vector3 startPos = transform.position;
        length = (int)(maxZ / chunkSizeZ);
        longLength = (int)(maxX / chunkSizeX);

        chunks = new Chunk[length, longLength];
        Debug.Log("Chunks size [" + length + ", " + longLength + "]");
        int count = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < longLength; j++)
            {
                Debug.Log("Created chunk [" + i + ", " + j + "]");
                chunks[i, j] = new Chunk("Chunk[" + i + ", " + j + "]",startPos.x + i * chunkSizeX, startPos.z + j * chunkSizeZ, startPos.x + i * chunkSizeX - chunkSizeX, startPos.z + j * chunkSizeZ - chunkSizeZ);
                count++;
            }
        }
        Debug.Log("Created " + count + " chunks.");
    }

    void Search(Chunk chunk)
    {
        List<MeshRenderer> keys = new List<MeshRenderer>();
        foreach (var mesh in meshCollection)
        {
            var pos = mesh.Key.transform.position;
            if (chunk.maxX > pos.x && pos.x > chunk.minX && chunk.maxZ > pos.z && pos.z > chunk.minZ)
            {
                chunk.filters.Add(mesh.Value, mesh.Key.sharedMaterial);
                keys.Add(mesh.Key);
            }
        }
        for (int i = 0; i < keys.Count; i++)
        {
            meshCollection.Remove(keys[i]);
        }
    }

    void CreateMeshChunk(Chunk chunk)
    {
        var filters = chunk.filters;
        for (int i = 0; i < materials.Length; i++)
        {
            Transform parent = new GameObject(chunk.name).transform;
            meshCombiner.parent = parent;
            List<MeshFilter> selectedFilters = new List<MeshFilter>();
            foreach (var filter in filters)
            {
                if(filter.Value == materials[i])
                {
#if UNITY_EDITOR
                    if(PrefabUtility.IsPartOfPrefabInstance(filter.Key.transform.parent.gameObject))
                    {
                        PrefabUtility.UnpackPrefabInstance(filter.Key.transform.parent.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    }

#endif
                    filter.Key.transform.SetParent(parent);

                    selectedFilters.Add(filter.Key);
                }            
            }
            if(selectedFilters.Count > 0)
            {
                meshCombiner.meshFilters = selectedFilters.ToArray();
                var obj = meshCombiner.Merge();
                if(obj!= null)
                {
                    obj.AddComponent<MeshCollider>();
                }
            }
            else
            {
                Destroy(parent.gameObject);
            }

        }

    }

    class Chunk
    {
        public Chunk(string name, float maxX, float maxZ, float minX, float minZ)
        {

            this.name = name;
            this.maxX = maxX;
            this.maxZ = maxZ;
            this.minX = minX;
            this.minZ = minZ;
            filters = new Dictionary<MeshFilter, Material>();
            Debug.Log("Given name: " + name);
        }
        public string name; 
        public float maxX;
        public float maxZ;
        public float minX;
        public float minZ;

        public Dictionary<MeshFilter, Material> filters;

    }

#if UNITY_EDITOR
    public bool drawGrid;
    public float y = 0;
    public float gizmoHeight = 5;
    private void OnDrawGizmos()
    {
        if (!drawGrid) return;
        Vector3 startPos = transform.position;
        int lnth = (int)(maxZ / chunkSizeZ);
        int lnthLong = (int) (maxX / chunkSizeX);
        for (int i = 0; i < lnth; i++)
        {
            for (int j = 0; j < lnthLong; j++)
            {
                Gizmos.DrawWireCube(startPos + new Vector3(i * chunkSizeX - chunkSizeX / 2, y, j * chunkSizeX - chunkSizeX / 2), new Vector3(chunkSizeX, gizmoHeight, chunkSizeZ));
            }
        }
    }
#endif


}