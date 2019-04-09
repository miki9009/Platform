using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class Settings : DataContainer
{
    public Container data;

    //public Sett data;
    public override IData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = (Container)value;
        }
    }

    private void Start()
    {
        //DataManager.Loaded += SetResolution;
    }

    void SetResolution()
    {
       // Screen.SetResolution((int)data.resolution.x, (int)data.resolution.y, true);
       // DataManager.Loaded -= SetResolution;
    }

    [Serializable]
    public class Container : IData
    {
        public override string ToString()
        {
            return id;
        }
        public string id;
        public string ID
        {
            get
            {
                return id;
            }
        }
        public Float2 resolution = new Float2(1920, 1080);
        public bool runBenchmark;
        public bool firstStart;
        public bool buttonMovement;
        public bool showFps;
        public int graphicsLevel;
        public bool lowFrameRate;
    }
}
