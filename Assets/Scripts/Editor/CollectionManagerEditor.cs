using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CollectionManager))]
public class CollectionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (CollectionManager)target;

        if (GUILayout.Button("Print Collection"))
        {
            string collectionString = "";
            var collections = script.AllCollections();
            foreach (var set in collections)
            {
                //collectionString += collection.Key + ": " + collection.Value.Collection.Values + "\n";
                collectionString = "ID: " + set.Key + "\n";
                foreach (var item in set.Value.Collection)
                {
                    collectionString += item.Key + ": " + item.Value + "\n";
                }
                Debug.Log("Collections: \n" + collectionString);
            }

        }
    }
}