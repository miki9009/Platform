using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class CollectionsContainer : DataContainer
{
    public Container data;
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

    [Serializable]
    public class Container : IData
    {
        public string id;
        public string ID
        {
            get
            {
                return id;
            }
        }

        public int coins;
        public int emmeralds;
        public int goldKeys;
        public int silverKeys;
        public int bronzeKeys;
        public int restarts;
    }



}

