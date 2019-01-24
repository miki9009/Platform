using UnityEngine;
using UnityEditor;

namespace Engine.Props
{
    [CustomEditor(typeof(PropsPainter))]
    public class PropsPainterEditor : Editor
    {
        PropsPainter script;
        bool pressed;
        public void OnSceneGUI()
        {
            if (!Camera.current || script == null) return;

            var cam = Camera.current;
            script.camPos = GetMouseEditor(cam, script.collisionLayer, QueryTriggerInteraction.Ignore);

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            Event e = Event.current;

            //Check the event type and make sure it's left click.
            if (((e.type == EventType.MouseDrag && script.continues) || e.type == EventType.MouseDown) && e.button == 0)
            {
                if(!pressed)
                {
                    script.CreateProp();
                }
                pressed = true;
                e.Use();  //Eat the event so it doesn't propagate through the editor.
            }
            else
            {
                if(pressed)
                {
                    pressed = false;
                    script.CheckEmptyProps();
                }
            }
            
        }

        public override void OnInspectorGUI()
        {     
            base.OnInspectorGUI();
            script = (PropsPainter)target;           
        }


        Vector3 GetMouseEditor(Camera cam, int layer, QueryTriggerInteraction trigerInteraction)
        {
            var editorRect = EditorWindow.focusedWindow.position;
            var windowSize = editorRect.size;
            Vector2 rawMouseInput = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            var mouseInput = new Vector2(rawMouseInput.x, -rawMouseInput.y + editorRect.size.y + editorRect.position.y);
            Ray ray = cam.ScreenPointToRay(mouseInput);
            RaycastHit groundHit;

            if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, layer, trigerInteraction))
            {
                script.target = groundHit.collider.gameObject;
                script.normal = groundHit.normal;
                if(script.useNormalRotation)
                {
                    script.prefabRotation = (Quaternion.LookRotation(script.normal)).eulerAngles + new Vector3(90,0,0) + script.propPrefab.gameObject.transform.localEulerAngles;
                }
                return groundHit.point;
            }
            return new Vector3(0, 0, 0);
        }

    }
}
