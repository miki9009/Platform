using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public class Pause : MonoBehaviour
    {
        public Transform excludedRoot;
        public UnityEvent OnPauseEnter;
        public UnityEvent OnPauseLeave;

        protected Dictionary<int, bool> monoObjectsEnable;
        protected Dictionary<int, MonoBehaviour> monoObjects;

        public static Pause Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GameManager.GameFinished += PauseEnter;
        }

        private void OnDestroy()
        {
            GameManager.GameFinished -= PauseEnter;
        }

        public void PauseEnter()
        {
            Time.timeScale = 0;
            MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
            monoObjects = new Dictionary<int, MonoBehaviour>();
            monoObjectsEnable = new Dictionary<int, bool>();
            foreach (var script in scripts)
            {
                if (script.transform.root != excludedRoot)
                {
                    monoObjects.Add(script.GetHashCode(), script);
                    monoObjectsEnable.Add(script.GetHashCode(), script.enabled);
                    script.enabled = false;
                }
            }
            if (OnPauseEnter != null)
            {
                OnPauseEnter.Invoke();
            }
        }

        public void PauseLeave()
        {
            Time.timeScale = 1;
            foreach (var valuePair in monoObjects)
            {
                if (valuePair.Value != null)
                {
                    valuePair.Value.enabled = monoObjectsEnable[valuePair.Key];
                }
            }
            if (OnPauseLeave != null)
            {
                OnPauseLeave.Invoke();
            }
        }

    }
}