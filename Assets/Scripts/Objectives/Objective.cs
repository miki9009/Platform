using UnityEngine;
using System;
using UnityEngine.Events;

namespace Objectives
{
    [Serializable]
    public class Objective
    {
        public string title;
        public bool optional;
        
        public bool IsFinished { get; set; }
        public virtual float Progress { get; private set; }
        [HideInInspector]
        public State state;
        public event Action<Objective> Started;
        public event Action<Objective> ProgressUpdated;
        public event Action<Objective> Finished;

        public virtual void Cancel()
        {
            IsFinished = true;
            state = State.Failed;
        }

        public virtual void ObjectiveStart()
        {
        }

        protected void OnStart()
        {
            Started?.Invoke(this);
        }

        protected void OnProgressUpdated()
        {
            ProgressUpdated?.Invoke(this);
        }

        protected virtual void OnFinished()
        {
            IsFinished = true;
            ObjectivesManager.OnObjectiveEnded(this);
            Finished?.Invoke(this);
        }

        public void Failed()
        {
            state = State.Failed;
            OnFinished();
            if (!optional && !ObjectivesManager.EndingGame)
            {
                ObjectivesManager.EndGame(GameManager.GameState.Failed);
            }
        }

    }

    public enum State
    {
        InProgress,
        Failed,
        Completed
    }
}