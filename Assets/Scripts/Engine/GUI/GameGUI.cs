using Engine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine
{
    [DefaultExecutionOrder(-75)]
    [RequireComponent(typeof(CanvasScaler))]
    public class GameGUI : MonoBehaviour
    {
        public static List<Engine.UI.Button> buttons = new List<Engine.UI.Button>();
        public static GameGUI Instance { get; private set; }

        public delegate void GUITouch(Vector3 pos);
        public static event GUITouch OnTouch;

        CanvasScaler scalar;

        public Vector2 mousePos;

        public Vector2 resolution;

        public static Vector2 ScreenScale
        {
            get; private set;
        }

        RectTransform rect;

        public Vector2 scale;

        public bool onGUI;

        private static readonly Vector2 outOfBounds = new Vector2(-100000, -100000);

        bool[] buttonIsInRange;

        private void Awake()
        {
            Debug.Log("GameGUI Instance Awake");
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("GameGUI already exists!");
                Destroy(gameObject);
            }

            scalar = GetComponent<CanvasScaler>();
            scalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scalar.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            resolution = new Vector2(Screen.width, Screen.height);
            rect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            Input.multiTouchEnabled = true;
            buttonIsInRange = new bool[1];
        }

        private void Update()
        {
            mousePos = Input.mousePosition;
            ScreenScale = rect.localScale;
            var touches = Input.touches;
            int touchCount = touches.Length;
            if (!Application.isPlaying) return;

            int buttonsCount = buttons.Count;

            if (buttonIsInRange.Length != buttonsCount)
            {
                buttonIsInRange = new bool[buttonsCount];
            }

#if UNITY_EDITOR
            bool mousePressed = Input.GetMouseButton(0);
            for (int l = 0; l < buttonsCount; l++)
            {
                if (mousePressed && buttons[l].IsInRange(Input.mousePosition))
                {
                    
                    buttons[l].Touch();
                }
                else
                {
                    buttons[l].NotTouched();
                }
            }
#else

            for (int i = 0; i < touchCount; i++)
            {
                for (int j = 0; j < buttonsCount; j++)
                {
                    if (!buttonIsInRange[j])
                    {
                        buttonIsInRange[j] = buttons[j].IsInRange(touches[i].position);
                    }
                }
            }

            for (int k = 0; k < buttonsCount; k++)
            {
                if (buttonIsInRange[k])
                {
                    buttons[k].Touch();
                }
                else
                {
                    buttons[k].NotTouched();
                }
                buttonIsInRange[k] = false;
            }

#endif
        }

        public static Engine.UI.Button GetButtonByName(string buttonName)
        {
            try
            {
                return buttons.Single(x => x.buttonName == buttonName);
            }
            catch
            {
                return null;
            }
        }
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
            ScreenScale = rect.localScale;
        }
#endif
        //private void OnGUI()
        //{
        //    if (!onGUI) return;
        //    int y = 50;
        //    int touchCount = Input.touchCount;
        //    Draw.TextColor(10, y, 255, 0, 0, 1, "Number of touches: " + touchCount);
        //    y +=50;
        //    var touches = Input.touches;
        //    for (int i = 0; i < touchCount; i++)
        //    {
        //        var touch = touches[i];
        //        Draw.TextColor(10, y, 255, 0, 0, 1, touch.position);
        //        y += 50;
        //    }
        //}
    }

}




