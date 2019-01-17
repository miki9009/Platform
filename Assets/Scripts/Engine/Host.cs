using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Engine.Threads
{
    public sealed class Host
    {        
        private static List<Thread> threads;
        private object safeLock = new object();
        private Thread thread = null;
        public delegate void Finish(object sender);
        public event Finish OnFinished;
        public delegate void HostUpdate();
        HostUpdate hostUpdate;
        private long interval = 100;
        public string name;
        public bool debugMode;

        private bool isWaiting = false;
        private bool isUnityDependant = false;

        ManualResetEvent _event = new ManualResetEvent(true);

        public bool IsUnityDependant
        {
            get
            {
                return isUnityDependant;
            }
            set
            {
                isUnityDependant = value;
            }
        }

        public static List<Thread> Threads
        {
            get
            {
                return Threads;
            }
        }

        public long Interval
        {
            get
            {
                lock (safeLock)
                {
                    return interval;
                }
            }

            set
            {
                lock (safeLock)
                {
                    interval = value;
                }
            }
        }

        public Thread Thread
        {
            get
            {
                return thread;
            }
        }

        /// <summary>
        /// Create a new thread
        /// </summary>
        public Host()
        {
            HostInitializer.OnHostWait += ResetWait;
            thread = new Thread(Run);
            thread.Start();
        }

        void ResetWait()
        {
            isWaiting = false;
        }

        ~Host()
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }

        public void Abort()
        {
            if (thread != null)
            {
                thread.Abort();
                UnityEngine.Debug.Log("THREAD "+ name + " Aborted");
            }
        }

        public void AddMethod(HostUpdate method)
        {
            hostUpdate += method;
        }


        public void RemoveMethod(HostUpdate method)
        {
            hostUpdate -= method;
        }

        private void Run()
        {
            Stopwatch watch;
            while (true)
            {
                if (isUnityDependant && isWaiting)
                {


                }

                lock (safeLock)
                {
                    isWaiting = true;
                    watch = Stopwatch.StartNew();
                    hostUpdate?.Invoke();
                    watch.Stop();
                    long elapsedMs = watch.ElapsedMilliseconds;
                    OnFinished?.Invoke(this);
                    int wait = 0;
                    if (elapsedMs < interval)
                    {
                        wait = (int)(interval - elapsedMs);
                    }
                    else
                    {
                        if (debugMode)
                        {
                            UnityEngine.Debug.Log("Host " + name + " did not manage to execute in given interval, elapsed miliseconds: " + elapsedMs);
                        }
                    }

                    if (!IsUnityDependant)
                    {
                        Monitor.Wait(safeLock, wait);
                    }
                    else
                    {
                        Monitor.Wait(safeLock, 16);
                    }
 
                }
            }           
        }
    }
}
