using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
    public enum AIState
    {
        Idle,
        Waypoints,
        Collection
    }

    [Serializable]
    public class AIBehaviour
    {
        public CharacterMovementAI characterMovementAi;
        public Func<bool> Execute;
        AIBehaviourState currentState;
    
        public bool Idle
        {
            get
            {
                return currentState.State == AIState.Idle;
            }
        }

        public Vector3 Destination
        {
            get
            {
                return currentState.destination;
            }
        }

        public AIBehaviour(CharacterMovementAI characterMovementAi)
        {
            currentState = new AIIdle(AIState.Idle, characterMovementAi);
            this.characterMovementAi = characterMovementAi;
        }

        public void AssignState(AIState state)
        {
            if (currentState != null && currentState.State == state) return;
            switch (state)
            {
                case AIState.Waypoints:
                    currentState = new AIWaypoint(state, characterMovementAi);
                    Execute = currentState.Execute;
                    break;
                case AIState.Collection:
                    currentState = new AICollection(state, characterMovementAi);
                    Execute = currentState.Execute;
                    break;
            }
        }
    }

    [Serializable]
    abstract class AIBehaviourState
    {
        public abstract bool Execute();
        protected abstract void Initialize();
        public CharacterMovementAI CharacterMovement { get; }
        public AIState State { get; }
        public Vector3 destination;
        protected bool initialized;
        protected AIBehaviour Behaviour { get; private set; }
        public AIBehaviourState(AIState state, CharacterMovementAI character)
        {
            destination = character.transform.position;
            CharacterMovement = character;
            Initialize();
            Behaviour = character.aIBehaviour;
            State = state;
        }
    }

    [Serializable]
    class AIIdle : AIBehaviourState
    {
        public AIIdle(AIState state, CharacterMovementAI character) : base(state, character){ }
        protected override void Initialize(){}
        public override bool Execute(){ return true; }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////WAYPOINTS/////////////////////////////////////////////////////
    /// </summary>
    [Serializable]
    class AIWaypoint : AIBehaviourState
    {
        public AIWaypoint(AIState state, CharacterMovementAI character) : base(state, character){}

        List<Waypoint> waypoints;
        public int waypointIndex;
        Waypoint waypoint;

        protected override void Initialize()
        {
            if (WaypointManager.Instance == null || !WaypointManager.Instance.waypoints.ContainsKey(waypointIndex)) return;
            initialized = true;
            waypoints = WaypointManager.Instance.waypoints.Values.ToList();
            waypoints.Sort((x, y) => x.index.CompareTo(y.index));
            waypoint = waypoints[waypointIndex];
            waypoint.Visited += WaypointVisited;
            destination = waypoint.transform.position;
        }

        void WaypointVisited(CharacterMovement characterMovement)
        {
            if (characterMovement.GetType() != typeof(CharacterMovementAI)) return;
            var characterMovementAi = (CharacterMovementAI)characterMovement;
            if(characterMovementAi.aIBehaviour == Behaviour)
            {
                waypoint.Visited -= WaypointVisited;
                if (waypointIndex + 1 < waypoints.Count)
                    waypointIndex++;
                else
                    waypointIndex = 0;
                waypoint = waypoints[waypointIndex];
                waypoint.Visited += WaypointVisited;

                destination = waypoint.transform.position;
            }
        }

        public override bool Execute()
        {
            return true;
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////COLLECTIONS//////////////////////////////////////////
    /// </summary>
    [Serializable]
    class AICollection : AIBehaviourState
    {
        public AICollection(AIState state, CharacterMovementAI character) : base(state, character)
        {
            transform = character.transform;
        }

        Transform transform;
        float time = 1;

        CollectionObject collection;

        public override bool Execute()
        {
            if (SignificantCollection.Count == 0) return false;
            if(collection == null)
            {
                Refresh();
            }
            else
            {
                if(!collection.enabled || !collection.gameObject.activeInHierarchy)
                {
                    collection = null;
                }
            }
            if(time > 0)
            {
                time -= 0.016f;
            }
            else
            {
                time = 1;
                Refresh();
            }
            return true;
        }

        void Refresh()
        {
            collection = SignificantCollection.FindNearest(transform.position);
            //Debug.Log("Collection: " + collection.GetInstanceID());
            if (collection != null)
            {
                destination = collection.transform.position;
                CharacterMovement.path =  CharacterMovement.pathMovement.GetPath(destination);
            }
        }

        protected override void Initialize()
        {
            CollectionManager.Instance.Collected += OnCollected;
        }

        void OnCollected(int id, CollectionType collection, int amount)
        {
            if(id == CharacterMovement.character.ID)
            {
                SignificantCollection.Remove(this.collection);
                this.collection = null;
            }
        }
    }
}