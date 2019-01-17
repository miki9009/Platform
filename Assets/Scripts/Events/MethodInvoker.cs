using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Events
{
    class MethodInvoker : MonoBehaviour
    {
        public List<Method> methods;
    }

    [Serializable]
    public class Method
    {
        [MethodsDrawer]
        public string method;
    }
}
