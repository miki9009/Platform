using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

namespace Engine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class Button : MonoBehaviour
    {
        public enum ButtonShape { Circle, Rectangle}
        public string buttonName;
        [HideInInspector]public float radius;
        public bool isTouched;
        RectTransform rect;
        Image image;
        Colour color;
        private bool wasAwaken = false;
        public ButtonShape buttonShape;

        public UnityEvent OnTapPressed;
        public UnityEvent OnTapContinue;
        public UnityEvent OnTapRelesed;
        public UnityEvent OnDoubleTap;

        float timerDoubleTap = 0;
        float interval = 0.25f;

        float dis;

        float width = 0;
        float height = 0;
        float x;
        float y;

        private void Awake()
        {
            if (!Application.isPlaying) return;
            GameGUI.buttons.Add(this);
        }

        protected void OnEnable()
        {
            if (!wasAwaken)
            {
                rect = GetComponent<RectTransform>();
                image = GetComponent<Image>();
                if (image != null)
                {
                    color = image.color;
                }
                wasAwaken = false;
            }
            radius = rect.rect.width / 2;
            width = (rect.rect.width / 2);
            height = (rect.rect.height / 2);
            x = rect.anchoredPosition.x;
            y = rect.anchoredPosition.y;

        }

        private void Start()
        {
            if (buttonShape == ButtonShape.Rectangle)
            {

            }


        }

        public void OnTapPressedInvoke()
        {
            if(OnTapPressed!= null)
            {
                OnTapPressed.Invoke();
            }
        }


        public bool IsInRange(Vector3 pos)
        {
            if (buttonShape == ButtonShape.Circle)
            {
                return Vector3.Distance(pos, rect.position) < (radius * GameGUI.ScreenScale.x);
            }
            else
            {
                //return ((pos.x > x - width * GameGUI.ScreenScale.x && pos.x < x + width * GameGUI.ScreenScale.x) && (pos.y > y - height * GameGUI.ScreenScale.y && pos.y < y + height * GameGUI.ScreenScale.y));

                //Debug.Log(string.Format("x:{0}, y:{1}", Vector3.Distance(new Vector3(x, 0, 0), new Vector3(pos.x, 0, 0)), Vector3.Distance(new Vector3(0, y, 0), new Vector3(pos.y, 0, 0))));
                //return (Vector3.Distance(new Vector3(x,0,0), new Vector3(pos.x, 0, 0)) < (width * GameGUI.ScreenScale.x)) 
                //    && (Vector3.Distance(new Vector3(0, y, 0), new Vector3(pos.y, 0, 0)) < height * (GameGUI.ScreenScale.y));
                Debug.Log("x: " + pos.x * GameGUI.ScreenScale.x);
                //Debug.Log(Vector3.Distance(Vector3.right * x, Vector3.right * pos.x * GameGUI.ScreenScale.x));
                

                return false;
            }
        }


        public void Touch()
        {
            if (!isTouched)
            {
                //if (buttonName == "ButtonAttack")
                //{
                //    Debug.Log("ActionButton Touched");
                //}
                OnTapPressed.Invoke();
            }
            isTouched = true;
            OnTapContinue.Invoke();
            if (timerDoubleTap > 0)
            {
                OnDoubleTap.Invoke();
            }            
        }

        public void NotTouched()
        {
            if (isTouched)
            {
                OnTapRelesed.Invoke();
                timerDoubleTap = interval;
            }
            isTouched = false;


            if (timerDoubleTap > 0)
            {
                timerDoubleTap -= Time.deltaTime;
            }
        }

#if UNITY_EDITOR
        [Header("DEBUG")]
        public float maskAlpha = 0.5f;
        public Color gizmosColor = Color.red;
        public bool showDebug;


        private void Update()
        {
            if (showDebug) yy = 50;
        }
        private static int yy;
        private void OnGUI()
        {
            if (!showDebug) return;
            Engine.Draw.TextColor(10, yy, 255, 0, 0, 1, buttonName + " IsTouched: " + isTouched);
            Engine.Draw.TextColor(10, yy + 20, 255, 0, 0, 1, "Dis: " + dis + " radius: " + (radius * GameGUI.ScreenScale.x));
            yy += 40;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = new Colour(gizmosColor.r, gizmosColor.g, gizmosColor.b, maskAlpha);
            if (buttonShape == ButtonShape.Circle)
            {
                Gizmos.DrawSphere(transform.position, radius * GameGUI.ScreenScale.x);
            }
            else
            {
                Gizmos.DrawCube(transform.position, new Vector3(width * 2 * GameGUI.ScreenScale.x, height * 2 * GameGUI.ScreenScale.y, 1));
            }
            
        }

#endif
    }


}
