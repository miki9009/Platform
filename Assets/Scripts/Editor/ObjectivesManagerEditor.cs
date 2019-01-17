using Engine;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

namespace Objectives
{
    [CustomEditor(typeof(ObjectivesManager))]
    public class ObjectivesManagerEditor : Editor
    {
        ObjectivesManager objectivesManager;

        bool toggleTxt = false;

        List<bool> foldout;
        string[] referencesPopup;
        int[] referenceLength;

        public override void OnInspectorGUI()
        {
            objectivesManager = (ObjectivesManager)target;
            if (objectivesManager.elementID == -1)
                objectivesManager.elementID = Level.GetID();

            if (foldout == null || foldout.Count != objectivesManager.sequence.sequences.Count)
            {
                int count = objectivesManager.sequence.sequences.Count;

                if (foldout == null)
                    foldout = new List<bool>();
                while(foldout.Count < count)
                {
                    foldout.Add(false);
                }
                while(foldout.Count > count)
                {
                    foldout.RemoveAt(foldout.Count - 1);
                }
            }

            Color defaultColor = GUI.backgroundColor;

            referencesPopup = new string[objectivesManager.levelElementReferences.Count];
            for (int i = 0; i < referencesPopup.Length; i++)
            {
                if(objectivesManager.levelElementReferences[i]!=null)
                    referencesPopup[i] = objectivesManager.levelElementReferences[i].elementID.ToString();
            }

            referenceLength = new int[objectivesManager.levelElementReferences.Count];

            Color col = new Color(0.5f, 0.7f, 0.8f);

            for (int i = 0; i < objectivesManager.sequence.sequences.Count; i++)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = col;
                foldout[i] = EditorGUILayout.Foldout(foldout[i], "SEQUENCE: " + i, true);
                if (foldout[i])
                {
                    var sequence = objectivesManager.sequence.sequences[i];

                    for (int j = 0; j < sequence.objectives.Count; j++)
                    {
                        EditorGUILayout.LabelField("Objective", EditorStyles.boldLabel);
                        var objective = sequence.objectives[j];
                        GUILayout.BeginVertical(EditorStyles.helpBox);

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Title", GUILayout.Width(50));
                        objective.title = GUILayout.TextField(objective.title, GUILayout.Width(250), GUILayout.MinWidth(50));
                        objective.triggerSequence = GUILayout.Toggle(objective.triggerSequence, "Trigger Sequence");
                        objective.optional = GUILayout.Toggle(objective.optional, "Is Optional");
                        if (GUILayout.Button("X", GUILayout.Width(35)))
                        {
                            sequence.objectives.Remove(objective);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Collection Type: ", GUILayout.Width(100));
                        objective.collectionType =(CollectionType) EditorGUILayout.EnumPopup(objective.collectionType, GUILayout.MinWidth(150), GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField("Amount: ", GUILayout.Width(70));
                        objective.collectionAmount = EditorGUILayout.IntField(objective.collectionAmount);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                        GUILayout.EndVertical();
                        GUILayout.BeginHorizontal();
                        objective.isTimer = GUILayout.Toggle(objective.isTimer, "Is Timer");
                        if(objective.isTimer)
                        {
                            EditorGUILayout.LabelField("Time: ", GUILayout.Width(100));
                            objective.time = EditorGUILayout.FloatField(objective.time);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        objective.triggerObject = GUILayout.Toggle(objective.triggerObject, "Trigger object:", GUILayout.Width(100));
                        int referenceIndex = 0;
                        if(objective.triggerObject)
                        {
                            if(objective.references!=-1)
                            {
                                bool found = false;
                                for (int k = 0; k < referencesPopup.Length; k++)
                                {
                                    if (objective.references.ToString() == referencesPopup[k])
                                    {
                                        found = true;
                                        referenceIndex = k;
                                        break;
                                    }
                                }
                                if (!found) objective.references = -1;
                            }
                            EditorGUILayout.LabelField("Reference Object: ", GUILayout.Width(150));
                            referenceIndex = EditorGUILayout.Popup(referenceIndex, referencesPopup, EditorStyles.popup);
                            if(referenceIndex < referencesPopup.Length && referencesPopup[referenceIndex] != null)
                                objective.references = int.Parse(referencesPopup[referenceIndex]);
                        }

                        GUILayout.EndHorizontal();
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Objective"))
                    {
                        sequence.objectives.Add(new CollectionObjective());
                    }
                    if (GUILayout.Button("Remove Sequence"))
                    {
                        objectivesManager.sequence.sequences.Remove(sequence);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
            if (GUILayout.Button("Add Sequence"))
            {
                objectivesManager.sequence.sequences.Add(new Sequence());
            }

#if UNITY_EDITOR
            if(objectivesManager.catchReferences)
            {
                objectivesManager.CatchReferences();
                objectivesManager.catchReferences = false;
            }
#endif

            var references = objectivesManager.levelElementReferences;
            for (int i = 0; i < references.Count; i++)
            {
                references[i] = (LevelElement)EditorGUILayout.ObjectField(references[i], typeof(LevelElement), true);
            }
            if (GUILayout.Button("Add ObjectField"))
            {
                references.Add(null);
            }
            objectivesManager.returnToVillage = GUILayout.Toggle(objectivesManager.returnToVillage, "Return to Village on complete");


            GUI.backgroundColor = defaultColor;

    

        }
    }
}