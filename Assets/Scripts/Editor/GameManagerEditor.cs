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
    bool[] characterCollectionsFoldout;
    bool[] characterGameProgressFoldout;

    public override void OnInspectorGUI()
    {
        gameManager = (GameManager)target;
        if (!GameManager.Instance)
        {
            EditorGUILayout.LabelField("Preview only available in Play Mode");
            return;
        }

        if (!styleInit)
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
            characterCollectionsFoldout = new bool[Character.allCharacters.Count];
            characterGameProgressFoldout = new bool[Character.allCharacters.Count];
            characterColor = new Color32(177, 190, 198, 1);
        }

        foldout[0] = EditorGUILayout.Foldout(foldout[0], "GAMEPLAY", true, EditorStyles.toolbarButton);

        if (foldout[0])
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
            EditorGUILayout.LabelField("Custom Scene Name: ", style2, GUILayout.MinWidth(50));
            EditorGUILayout.LabelField(GameManager.CustomScene, style, GUILayout.MinWidth(50));
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

            //CustomLevel
            GUI.backgroundColor = col;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MissionID: ", style2, GUILayout.MinWidth(50));
            if(GameManager.CurrentMission != null)
                 EditorGUILayout.LabelField(GameManager.CurrentMission.ToString(), style, GUILayout.MinWidth(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultColor;

            //CustomLevel
            GUI.backgroundColor = col;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mission Index: ", style2, GUILayout.MinWidth(50));
            if (GameManager.CurrentMission != null)
                EditorGUILayout.LabelField(GameManager.MissionIndex.ToString(), style, GUILayout.MinWidth(50));
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
                        EditorGUILayout.ObjectField(character, typeof(Character),true);
                        EditorGUILayout.EndHorizontal();

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

                        characterCollectionsFoldout[foldoundIndex] = EditorGUILayout.Foldout(characterCollectionsFoldout[foldoundIndex], "Collections:", true, EditorStyles.toolbarButton);
                        if (characterCollectionsFoldout[foldoundIndex])
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

                        characterGameProgressFoldout[foldoundIndex] = EditorGUILayout.Foldout(characterGameProgressFoldout[foldoundIndex], "Game Progress:", true, EditorStyles.toolbarButton);
                        if (characterGameProgressFoldout[foldoundIndex])
                        {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            EditorGUILayout.LabelField("Placement: ", style2, GUILayout.MinWidth(50));
                            EditorGUILayout.LabelField(character.gameProgress.Placement.ToString(), style, GUILayout.MinWidth(50));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            EditorGUILayout.LabelField("Race Progress: ", style2, GUILayout.MinWidth(50));
                            EditorGUILayout.LabelField(character.gameProgress.raceProgress.ToString(), style, GUILayout.MinWidth(50));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();


                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            EditorGUILayout.LabelField("Waypoint: ", style2, GUILayout.MinWidth(50));
                            EditorGUILayout.LabelField(character.gameProgress.CurrentWaypoint.ToString(), style, GUILayout.MinWidth(50));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
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

    }
}
