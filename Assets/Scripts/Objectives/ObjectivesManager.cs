using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Objectives
{
    public class ObjectivesManager : LevelElement
    {
        public static List<Objective> Objectives { get; private set; }

        public List<Objective> activeObjectives = new List<Objective>();
        public ObjectiveSequence sequence;
        public List<LevelElement> levelElementReferences;

        public static ObjectivesManager Instance { get; private set; }
        
        public static bool EndingGame { get; private set; }
        public bool returnToVillage;
        int sequenceIndex = 0;


        int[] references;

#if UNITY_EDITOR
        public bool catchReferences = true;
#endif

        private void Awake()
        {
            Instance = this;
            Controller.Instance.PlayerDead += CancelObjectives;
        }

        void CancelObjectives(Character character)
        {
            foreach (var objective in Objectives)
                objective.Cancel();
        }

        private void Start()
        {
            Objectives = new List<Objective>();
            foreach (var sequence in sequence.sequences)
            {
                for (int i = 0; i < sequence.objectives.Count; i++)
                {
                    Objectives.Add(sequence.objectives[i]);
                }
            }
            TriggerSequence();
            Level.LevelLoaded += CatchReferences;
        }

        void TriggerSequence()
        {
            activeObjectives = new List<Objective>();

            if(sequenceIndex >= sequence.sequences.Count)
            {
                Debug.LogError("Sequence index grater than sequences count. Add sequence this is an error");
                return;
            }
            for (int i = 0; i < sequence.sequences[sequenceIndex].objectives.Count; i++)
                activeObjectives.Add(sequence.sequences[sequenceIndex].objectives[i]);

            foreach (var objective in activeObjectives)
            {
                ObjectivesPanelManager.AddPanel(objective);
                objective.ObjectiveStart();
            }
            sequenceIndex++;
        }

        public static event System.Action<Objective> ObjectiveEnded;
        public static void OnObjectiveEnded(Objective objective)
        {
            bool allObjectivesCompleted = true;
            foreach (var o in Objectives)
            {
                if (!o.IsFinished && !o.optional)
                {
                    allObjectivesCompleted = false;
                    break;
                }
            }

            if (allObjectivesCompleted)
            {
                if(Instance.returnToVillage)
                {
                    EndGame(GameManager.GameState.Completed);
                   // LevelManager.ChangeLevel("Village", "0");
                }

            }

            //GameManager.Instance.EndGame(GameManager.GameState.Completed);

            ObjectiveEnded?.Invoke(objective);
                        if (((CollectionObjective)objective).triggerSequence)
                Instance.TriggerSequence();
        }

        public override void OnSave()
        {
            base.OnSave();

            references = new int[levelElementReferences.Count];
            for (int i = 0; i < levelElementReferences.Count; i++)
            {
                references[i] = levelElementReferences[i].elementID;
            }
            if (data != null)
            {
                data["Sequence"] = sequence;
                data["References"] = references;
                data["ReturnVillage"] = returnToVillage;
            }

        }

        public override void OnLoad()
        {
            base.OnLoad();
            if(data!=null)
            {
                if(data.ContainsKey("ReturnVillage"))
                {
                    returnToVillage = (bool)data["ReturnVillage"];
                }
                if(data.ContainsKey("Sequence"))
                {
                    sequence = (ObjectiveSequence)data["Sequence"];
                }
                if (data.ContainsKey("References"))
                {
                    references = (int[])data["References"];
                }
#if UNITY_EDITOR
                catchReferences = true;
#endif
            }
        }

        public static void EndGame(GameManager.GameState state)
        {
            if (GameManager.State != GameManager.GameState.Idle)
            {
                Debug.Log("End state already called");
                return;
            }
            CoroutineHost.Start(EndGameCor(state));
        }

        public static IEnumerator EndGameCor(GameManager.GameState state, float time = 1)
        {           
            EndingGame = true;
            yield return new WaitForSeconds(time);
            GameManager.Instance.EndGame(state);
            EndingGame = false;
        }

        private void OnDestroy()
        {
            Level.LevelLoaded -= CatchReferences;
            Controller.Instance.PlayerDead -= CancelObjectives;
        }

        public void CatchReferences()
        {
            levelElementReferences = new List<LevelElement>();
            if (references == null) return;
            for (int i = 0; i < references.Length; i++)
            {
                if (Level.loadedElements.ContainsKey(references[i]))
                    levelElementReferences.Add(Level.loadedElements[references[i]]);
            }
        }

    }
}