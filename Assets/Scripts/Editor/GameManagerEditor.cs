using Engine;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;


    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        GameManager gameManager;

        bool toggleTxt = false;

        string[] referencesPopup;
        int[] referenceLength;
        Color lime = new Color(0.72f, 1, 0.223f);
        Color characterColor;
        Color defaultColor;
        Color col;
        GUIStyle style;
        GUIStyle style2;
        bool styleInit;
        bool[] foldout;
        bool[] characterFoldouts;
        public override void OnInspectorGUI()
        {
            gameManager = (GameManager)target;
            if(!GameManager.Instance)
            {
                EditorGUILayout.LabelField("Preview only available in Play Mode");
                return;
            }

            if(!styleInit)
            {
                Debug.Log("Init style");
                styleInit = true;
                defaultColor = GUI.backgroundColor;
                col = new Color(0.5f, 0.7f, 0.8f);
                style = new GUIStyle();
                style.normal.textColor = lime;
                style.alignment = TextAnchor.MiddleRight;

                style2 = new GUIStyle();
                style2.normal.textColor = Color.white;
                foldout = new bool[30];
                characterFoldouts = new bool[20];
                characterColor = new Color32(177, 190, 198, 1);
            }

            foldout[0] = EditorGUILayout.Foldout(foldout[0], "GAMEPLAY" , true, EditorStyles.toolbarButton);

            if(foldout[0])
            {
                //GameState
                GUI.backgroundColor = col;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Game State: ", style2, GUILayout.MinWidth(50));
                EditorGUILayout.LabelField(GameManager.State.ToString(), style, GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Paused: ", style2, GUILayout.MinWidth(50));
                EditorGUILayout.LabelField(GameManager.IsPaused ? "True" : "False", style, GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = defaultColor;

                //Scene
                GUI.backgroundColor = col;


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Scene: ", style2, GUILayout.MinWidth(50));
                EditorGUILayout.LabelField(string.IsNullOrEmpty(GameManager.CurrentScene) ? "None" : GameManager.CurrentScene, style, GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                GUI.backgroundColor = defaultColor;

                //CustomLevel
                GUI.backgroundColor = col;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Custom Level Name: ", style2, GUILayout.MinWidth(50));
                EditorGUILayout.LabelField(GameManager.CustomLevelID, style, GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = defaultColor;

                //Mode
                GUI.backgroundColor = col;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Game Mode: ", style2, GUILayout.MinWidth(50));
                EditorGUILayout.LabelField(GameManager.GameMode, style, GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = defaultColor;
            }

            foldout[1] = EditorGUILayout.Foldout(foldout[1], "CONTROLLER", true, EditorStyles.toolbarButton);

        if (foldout[1])
        {
            if (Controller.Instance == null)
            {
                EditorGUILayout.LabelField("Controller does not exist");
            }
            else
            {
                //Characters
                GUI.backgroundColor = col;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Players Count: ", style2, GUILayout.MinWidth(50));
                EditorGUILayout.LabelField(Character.allCharacters.Count.ToString(), style, GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = defaultColor;
                foldout[2] = EditorGUILayout.Foldout(foldout[2], "Characters:", true);
                if (foldout[2])
                {
                    int foldoundIndex = 0;
                    foreach (var character in Character.allCharacters)
                    {
                        GUI.backgroundColor = Color.gray;
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.LabelField("ID: ", style2, GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField(character.ID.ToString(), style, GUILayout.MinWidth(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(EditorStyles.largeLabel);
                        EditorGUILayout.LabelField("Host: ", style2, GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField(character.IsHost ? "True" : "False", style, GUILayout.MinWidth(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.LabelField("Is Local: ", style2, GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField(character.IsLocalPlayer ? "True" : "False", style, GUILayout.MinWidth(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(EditorStyles.largeLabel);
                        EditorGUILayout.LabelField("Human: ", style2, GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField(character.IsBot ? "False" : "True", style, GUILayout.MinWidth(50));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.LabelField("Is Dead: ", style2, GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField(character.IsDead ? "True" : "False", style, GUILayout.MinWidth(50));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();

                        if (CollectionManager.Instance == null) continue;

                        characterFoldouts[foldoundIndex] = EditorGUILayout.Foldout(characterFoldouts[foldoundIndex], "Collections:", true, EditorStyles.toolbarButton);
                        if (characterFoldouts[foldoundIndex])
                        {
                            var collectionSet = CollectionManager.Instance.GetPlayerCollectionSet(character.ID);
                            if (collectionSet != null)
                            {
                                var collections = collectionSet.Collection;
                                foreach (var collection in collections)
                                {
                                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                    EditorGUILayout.LabelField(collection.Key.ToString(), style2, GUILayout.MinWidth(50));
                                    EditorGUILayout.LabelField(collection.Value.ToString(), style, GUILayout.MinWidth(50));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                EditorGUILayout.LabelField("Collection empty", style2, GUILayout.MinWidth(50));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                            }
                        }
                        GUI.backgroundColor = defaultColor;

                        foldoundIndex++;
                    }
                }


            }
        }

        //MULTIPLAYER
        foldout[3] = EditorGUILayout.Foldout(foldout[3], "MULTIPLAYER", true, EditorStyles.toolbarButton);

        if (foldout[3])
        {
            GUI.backgroundColor = col;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Multiplayer: ", style2, GUILayout.MinWidth(50));
            EditorGUILayout.LabelField(PhotonManager.IsMultiplayer ? "True" : "False", style, GUILayout.MinWidth(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Master Client: ", style2, GUILayout.MinWidth(50));
            EditorGUILayout.LabelField(PhotonManager.IsMaster ? "True" : "False", style, GUILayout.MinWidth(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Connected: ", style2, GUILayout.MinWidth(50));
            EditorGUILayout.LabelField(PhotonManager.IsConnected ? "True" : "False", style, GUILayout.MinWidth(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Players Count: ", style2, GUILayout.MinWidth(50));
            EditorGUILayout.LabelField(PhotonManager.Players.Count.ToString(), style, GUILayout.MinWidth(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultColor;
        }


        //            if (foldout == null || foldout.Count != gameManager.sequence.sequences.Count)
        //            {
        //                int count = gameManager.sequence.sequences.Count;

        //                if (foldout == null)
        //                    foldout = new List<bool>();
        //                while (foldout.Count < count)
        //                {
        //                    foldout.Add(false);
        //                }
        //                while (foldout.Count > count)
        //                {
        //                    foldout.RemoveAt(foldout.Count - 1);
        //                }
        //            }

        //            Color defaultColor = GUI.backgroundColor;

        //            referencesPopup = new string[gameManager.levelElementReferences.Count];
        //            for (int i = 0; i < referencesPopup.Length; i++)
        //            {
        //                if (gameManager.levelElementReferences[i] != null)
        //                    referencesPopup[i] = gameManager.levelElementReferences[i].elementID.ToString();
        //            }

        //            referenceLength = new int[gameManager.levelElementReferences.Count];

        //            Color col = new Color(0.5f, 0.7f, 0.8f);

        //            for (int i = 0; i < gameManager.sequence.sequences.Count; i++)
        //            {

        //                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //                GUI.backgroundColor = col;
        //                foldout[i] = EditorGUILayout.Foldout(foldout[i], "SEQUENCE: " + i, true);
        //                if (foldout[i])
        //                {
        //                    var sequence = gameManager.sequence.sequences[i];

        //                    for (int j = 0; j < sequence.objectives.Count; j++)
        //                    {
        //                        EditorGUILayout.LabelField("Objective", EditorStyles.boldLabel);
        //                        var objective = sequence.objectives[j];
        //                        GUILayout.BeginVertical(EditorStyles.helpBox);

        //                        GUILayout.BeginHorizontal();
        //                        GUILayout.Label("Title", GUILayout.Width(50));
        //                        objective.title = GUILayout.TextField(objective.title, GUILayout.Width(250), GUILayout.MinWidth(50));
        //                        objective.triggerSequence = GUILayout.Toggle(objective.triggerSequence, "Trigger Sequence");
        //                        objective.optional = GUILayout.Toggle(objective.optional, "Is Optional");
        //                        if (GUILayout.Button("X", GUILayout.Width(35)))
        //                        {
        //                            sequence.objectives.Remove(objective);
        //                        }
        //                        GUILayout.EndHorizontal();
        //                        GUILayout.BeginHorizontal();
        //                        EditorGUILayout.LabelField("Collection Type: ", GUILayout.Width(100));
        //                        objective.collectionType = (CollectionType)EditorGUILayout.EnumPopup(objective.collectionType, GUILayout.MinWidth(150), GUILayout.MinWidth(50));
        //                        EditorGUILayout.LabelField("Amount: ", GUILayout.Width(70));
        //                        objective.collectionAmount = EditorGUILayout.IntField(objective.collectionAmount);
        //                        GUILayout.EndHorizontal();
        //                        GUILayout.Space(10);
        //                        GUILayout.EndVertical();
        //                        GUILayout.BeginHorizontal();
        //                        objective.isTimer = GUILayout.Toggle(objective.isTimer, "Is Timer");
        //                        if (objective.isTimer)
        //                        {
        //                            EditorGUILayout.LabelField("Time: ", GUILayout.Width(100));
        //                            objective.time = EditorGUILayout.FloatField(objective.time);
        //                        }
        //                        GUILayout.EndHorizontal();
        //                        GUILayout.BeginHorizontal();
        //                        objective.triggerObject = GUILayout.Toggle(objective.triggerObject, "Trigger object:", GUILayout.Width(100));
        //                        int referenceIndex = 0;
        //                        if (objective.triggerObject)
        //                        {
        //                            if (objective.references != -1)
        //                            {
        //                                bool found = false;
        //                                for (int k = 0; k < referencesPopup.Length; k++)
        //                                {
        //                                    if (objective.references.ToString() == referencesPopup[k])
        //                                    {
        //                                        found = true;
        //                                        referenceIndex = k;
        //                                        break;
        //                                    }
        //                                }
        //                                if (!found) objective.references = -1;
        //                            }
        //                            EditorGUILayout.LabelField("Reference Object: ", GUILayout.Width(150));
        //                            referenceIndex = EditorGUILayout.Popup(referenceIndex, referencesPopup, EditorStyles.popup);
        //                            if (referenceIndex < referencesPopup.Length && referencesPopup[referenceIndex] != null)
        //                                objective.references = int.Parse(referencesPopup[referenceIndex]);
        //                        }

        //                        GUILayout.EndHorizontal();
        //                    }
        //                    GUI.backgroundColor = Color.white;
        //                    EditorGUILayout.BeginHorizontal();
        //                    if (GUILayout.Button("Add Objective"))
        //                    {
        //                        sequence.objectives.Add(new CollectionObjective());
        //                    }
        //                    if (GUILayout.Button("Remove Sequence"))
        //                    {
        //                        gameManager.sequence.sequences.Remove(sequence);
        //                    }
        //                    EditorGUILayout.EndHorizontal();
        //                }
        //                EditorGUILayout.EndHorizontal();
        //            }
        //            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
        //            if (GUILayout.Button("Add Sequence"))
        //            {
        //                gameManager.sequence.sequences.Add(new Sequence());
        //            }

        //#if UNITY_EDITOR
        //            if (gameManager.catchReferences)
        //            {
        //                gameManager.CatchReferences();
        //                gameManager.catchReferences = false;
        //            }
        //#endif

        //            var references = gameManager.levelElementReferences;
        //            for (int i = 0; i < references.Count; i++)
        //            {
        //                references[i] = (LevelElement)EditorGUILayout.ObjectField(references[i], typeof(LevelElement), true);
        //            }
        //            if (GUILayout.Button("Add ObjectField"))
        //            {
        //                references.Add(null);
        //            }
        //            gameManager.returnToVillage = GUILayout.Toggle(gameManager.returnToVillage, "Return to Village on complete");


        //            GUI.backgroundColor = defaultColor;



    }
    }
