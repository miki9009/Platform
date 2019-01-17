using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System;
using System.Linq;

namespace Engine.Threads
{
    [DefaultExecutionOrder(-100)]
    public class HostInitializer : MonoBehaviour
    {
        public static bool isWaiting;
        public List<HostInit> hostInits;

        private static HostInitializer instance;

        public static HostInitializer Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject("HostInitializer");
                     return obj.AddComponent<HostInitializer>();
                }
                return instance;
            }
        }

        public List<Host> hosts = new List<Host>();

        public Host DefaultHost
        {
            get
            {
                return GetHostByName("Default");
            }
            private set
            {

            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                if (DefaultHost == null)
                {
                    AddHost(16, "Default", false);
                }
            }
            else
            {
                Destroy(this);
            }
            StartCoroutine(UnityEndOframeCoroutine());
        }

        public delegate void HostWait();
        public static event HostWait OnHostWait;

        IEnumerator UnityEndOframeCoroutine()
        {
            while (true)
            {
                OnHostWait?.Invoke();
                yield return new WaitForEndOfFrame();
            }
        }

        private void Start()
        {
            if (hostInits != null && hostInits.Count > 0)
            {
                foreach (var host in hostInits)
                {
                    AddHost(host.interval, host.name, host.debugMode);
                }
            }
            Debug.Log("###HOSTS INITIALIZED");
        }

        public void AddHost(int interval, string _name, bool debug)
        {
            for (int i = 0; i < hosts.Count; i++)
            {
                if(hosts[i].name == _name)
                {
                    Debug.Log("Host with this name already exist");
                    return;
                }
            }
            var host = new Host()
            {
                Interval = interval,
                name = _name,
                debugMode = debug
            };
            hosts.Add(host);
            Debug.Log("Host '" + _name + "' created.");
        }

        public Host GetHostByName(string name)
        {
            if (hosts.Count == 0) return null;
            try
            {
                return hosts.Single(x => x.name == name);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            foreach (var host in hosts)
            {
                host.Abort();
            }
            GC.Collect();
        }

        [Serializable]
        public class HostInit
        {
            public string name;
            public int interval;
            public bool debugMode;
        }
    }
}
