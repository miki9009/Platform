using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


namespace Engine.Threads
{
    [DefaultExecutionOrder(-95)]
    public class MonoBehaviourHost : MonoBehaviour
    {
        [SerializeField] private List<IEnumerator> corutines;

        public List<MonoBehaviours> hosts;

        public static MonoBehaviourHost Instance { get; private set; }


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var host in hosts)
            {
                StartCoroutine(host.CoroutineUpdate(host.interval));
                UnityEngine.Debug.Log("MonoBehaviourHost initialized.");
            }
        }

        public MonoBehaviours GetHostByName(string name)
        {
            try
            {
                return hosts.Single(x => x.name == name);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
                return null;
            }
        }


        [Serializable]
        public class MonoBehaviours
        {
            [Range(0.01f, 10f)]
            public float interval;
            public string name;
            public bool debug;

            public delegate void HeavyUpdate();
            public List<HeavyUpdate> updates = new List<HeavyUpdate>();
            IEnumerator coroutine;



            public IEnumerator CoroutineUpdate(float seconds)
            {
                Stopwatch watch;
                var wait = new WaitForSeconds(seconds);
                while (true)
                {
                    watch = Stopwatch.StartNew();
                    if (updates != null && updates.Count > 0)
                    {

                        foreach (var method in updates)
                        {
                            watch.Reset();
                            if (watch.ElapsedMilliseconds < (seconds * 1000))
                            {
                                method();
                            }
                            else 
                            {
                                if (debug)
                                {
                                    UnityEngine.Debug.Log("Corutine did not manage to fit interval: " + watch.ElapsedMilliseconds / 1000f);
                                }
                                yield return wait;
                            }
                        }
                    }
                    yield return wait;
                }
            }

        }
    }
}
