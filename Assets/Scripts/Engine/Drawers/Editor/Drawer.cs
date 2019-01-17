

using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Engine
{
    [CustomPropertyDrawer(typeof(PopUpAttribute), true)]
    public class Drawer: PropertyDrawer
    {
        
        PopUpAttribute enumeration { get { return (PopUpAttribute)attribute; } }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var val = prop.stringValue;
            var array = enumeration.items;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == val)
                {
                    enumeration.selected = i;
                }
            }

            enumeration.selected = EditorGUI.Popup(EditorGUI.PrefixLabel(position, label), enumeration.selected, enumeration.items);
            prop.stringValue = enumeration.items[enumeration.selected];
            
        }

    }
}