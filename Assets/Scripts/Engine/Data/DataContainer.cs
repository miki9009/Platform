using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine
{
    [ExecuteInEditMode]
    public class DataContainer : MonoBehaviour
    {
        public virtual IData Data { get; set; }

    }



}
