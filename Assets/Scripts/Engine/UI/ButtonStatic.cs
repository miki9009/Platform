using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.UI
{
    public class ButtonStatic : MonoBehaviour
    {
        public List<Method> methods;

        List<Action> _ms;

        UnityEngine.UI.Button button;
        private void Awake()
        {
            button = GetComponent<UnityEngine.UI.Button>();
            if(button == null)
            {
                Debug.LogError("Button not attached to gameObject");
                return;
            }
            foreach(var method in methods)
            {
                AssignMethod(method.methodName);
            }
        }



        void AssignMethod(string method)
        {
            var s = method.Split('.');
            _ms = new List<Action>();
            var m = Type.GetType(s[0]).GetMethod(s[1]);

            _ms.Add(() =>
            {
                m.Invoke(null,null);
            });
        }

        public void InvokeMethods()
        {
            foreach(var method in _ms)
            {
                method.Invoke();
            }
        }

        [Serializable]
        public class Method
        {
            [StaticMethodDrawer]
            public string methodName;
        }
    }

    
}
