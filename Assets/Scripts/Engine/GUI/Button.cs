using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Engine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public string buttonName;
        [HideInInspector] public float radius;
        public bool Pressed
        {
            get
            {
                return pressed;
            }
        }
        RectTransform rect;
        Image image;
        Colour color;
        private bool wasAwaken = false;
        //public bool enabled = true;
        public UnityEvent OnTapPressed;
        public UnityEvent OnTapContinue;
        public UnityEvent OnTapRelesed;
        public UnityEvent OnDoubleTap;
        public float PressedTime { get; private set; }
        public KeyCode keyMap;

        private Vector2 touchPosition;
        public Vector2 TouchPosition
        {
            get
            {
                return touchPosition;
            }
        }


        float timerDoubleTap = 0;
        float interval = 0.25f;

        float dis;

        float width = 0;
        float height = 0;
        float x;
        float y;
        private bool pressed;

        private void Awake()
        {
            if (!Application.isPlaying) return;
            GameGUI.buttons.Add(this);
            if (!enabled)
                gameObject.SetActive(false);
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



        public void OnPointerDown(PointerEventData eventData)
        {
            if(!pressed)
            {
                Debug.Log("Pressed: " + name);
                pressed = true;
                touchPosition = eventData.position;
                OnTapPressed.Invoke();
            }

            touchPosition = eventData.position;
            OnTapContinue.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Released: " + name);
            pressed = false;
            touchPosition = eventData.position;
            OnTapRelesed.Invoke();
        }



        public void OnTapPressedInvoke()
        {
            if(OnTapPressed!= null)
            {                
                OnTapPressed.Invoke();
            }
        }


        public void Enable()
        {
            gameObject.SetActive(true);
            enabled = true;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            enabled = false;
        }


    }


}
