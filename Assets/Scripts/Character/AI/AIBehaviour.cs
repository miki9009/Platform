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
        AIBehaviourState _curState;
        AIBehaviourState CurrentState
        {
            get
            {
                return _curState;
            }

            set
            {
                if(_curState!=null)
                {
                    _curState.ClearForGC();
                }
                _curState = value;
            }
        }
    
        public bool Idle
        {
            get
            {
                return CurrentState.State == AIState.Idle;
            }
        }

        public Vector3 Destination
        {
            get
            {
                return CurrentState.destination;
            }
        }

        public AIBehaviour(CharacterMovementAI characterMovementAi)
        {
            CurrentState = new AIIdle(AIState.Idle, characterMovementAi);
            this.characterMovementAi = characterMovementAi;
        }

        public void AssignState(AIState state)
        {
            if (CurrentState != null && CurrentState.State == state) return;
            switch (state)
            {
                case AIState.Waypoints:
                    CurrentState = new AIWaypoint(state, characterMovementAi);
                    Execute = CurrentState.Execute;
                    break;
                case AIState.Collection:
                    CurrentState = new AICollection(state, characterMovementAi);
                    Execute = CurrentState.Execute;
                    break;
            }
        }

        public void Clear()
        {
            if (CurrentState != null)
                CurrentState.ClearForGC();
        }
    }

    [Serializable]
    abstract class AIBehaviourState
    {
        public abstract bool Execute();
        protected abstract void Initialize();
        public abstract void ClearForGC();
        public CharacterMovementAI AIMovement { get; }
        public AIState State { get; }
        public Vector3 destination;
        protected bool initialized;
        protected AIBehaviour Behaviour { get; private set; }
        public AIBehaviourState(AIState state, CharacterMovementAI character)
        {
            destination = character.transform.position;
            AIMovement = character;
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
        public override void ClearForGC()
        {
            //throw new NotImplementedException();
        }
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

        public override void ClearForGC()
        {
            if(waypoint)
            {
                waypoint.Visited -= WaypointVisited;
            }
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
        float refreshTime = 1;

        CollectionObject collection;

        public override bool Execute()
        {
            if (SignificantCollection.Count == 0) return false;
            if(collection == null)
            {
                Refresh();
                refreshTime = 1;
            }
            else
            {
                if(!collection.enabled || !collection.gameObject.activeInHierarchy)
                {
                    collection = null;
                }
            }
            if(refreshTime > 0)
            {
                refreshTime -= 0.016f;
            }
            else
            {
                refreshTime = 1;
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
                AIMovement.path =  AIMovement.pathMovement.GetPath(destination);
                if(AIMovement.path.Length == 1)
                {
                    collection.AINotReachable = true;
                    AIMovement.path = AIMovement.pathMovement.GetPath(destination);
                }
            }
        }

        protected override void Initialize()
        {
            CollectionManager.Instance.Collected += OnCollected;
            SignificantCollection.SignificantCollected += OnSingificantCollected;
        }

        void OnCollected(int id, CollectionType collection, int amount)
        {
            if(id == AIMovement.character.ID)
            {
                this.collection = null;
            }
        }

        void OnSingificantCollected(int id)
        {
            if(collection && id == collection.ID)
            {
                Refresh();
                refreshTime = 0;
            }
        }

        public override void ClearForGC()
        {
            if (!CollectionManager.Instance) return;
            CollectionManager.Instance.Collected -= OnCollected;
            SignificantCollection.SignificantCollected -= OnSingificantCollected;
        }
    }
}