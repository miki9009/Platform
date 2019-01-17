using System;
using UnityEngine;

namespace Engine
{
    [SerializableAttribute]
    public class PopUpAttribute : PropertyAttribute
    {

        public string[] items;
        public int selected = 0;

        public PopUpAttribute()
        {
        }

    }

}