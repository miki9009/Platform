using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Engine.Animation
{
    public class AnimationObject : MonoBehaviour
    {
        public int CurrentFrame
        {
            get
            {
                return (int)progress;
            }
        }
        public int frames;
        public float animationSpeed = 1;
        public List<TranslateState> states;
        public bool loop;

        float progress;

        private void Awake()
        {
            foreach (var state in states)
                state.Initialize();
        }

        private void Start()
        {

            Invoke("Play", 5);
        }

        private void OnGUI()
        {
                Engine.Draw.TextColorSize(10, 40, 255, 0, 0, 1, 30, states[0].keyFrames[states[0].currentKeyFrame].frameIndex);

        }

        IEnumerator animationEnumerator;

        IEnumerator Animation()
        {
            bool play = true;
            while (loop || play)
            {
                play = false;
                progress = 0;
                foreach (var state in states)
                {
                    state.Restart();
                }

                while (frames > CurrentFrame)
                {
                    for (int i = 0; i < states.Count; i++)
                    {
                        states[i].Evaluate(CurrentFrame);
                    }
                    progress += animationSpeed;
                    yield return null;
                }

                yield return null;
            }
                animationEnumerator = null;
        }

        public void Stop()
        {
            if (animationEnumerator != null)
                StopCoroutine(animationEnumerator);
            progress = 0;
        }

        public void Pause()
        {
            if (animationEnumerator != null)
                StopCoroutine(animationEnumerator);
        }

        public void Resume()
        {
            if (animationEnumerator != null)
                StopCoroutine(animationEnumerator);

            animationEnumerator = Animation();
            StartCoroutine(animationEnumerator);
        }

        public void Play()
        {
            progress = 0;
            Resume();
        }

    }

    [Serializable]
    public class AnimationKeyFrame
    {
        public int frameIndex;
        public AnimationCurve curve;
    }

    [Serializable]
    public class TranslationKeyFrame : AnimationKeyFrame
    {
        public Vector3 endPos;
    }

    [Serializable]
    public class AnimationState
    {
        //[NonSerialized]
        public int currentKeyFrame;

        //[NonSerialized]
        protected int currentKeyFrameIndex;
        //[NonSerialized]
        protected int keyIndex;

        public virtual AnimationKeyFrame[] KeyFrames { get;}
        public virtual void Evaluate(int frame) { }
        public virtual void Restart() { }

        public virtual void Initialize() { }

        public AnimationKeyFrame GetNextKeyFrame(int frame)
        {
            for (int i = 0; i < KeyFrames.Length; i++)
            {
                if (frame < KeyFrames[i].frameIndex)
                {
                    keyIndex = i;
                    return KeyFrames[i];
                }
            }

            return KeyFrames[0];
        }
    }

    [Serializable]
    public class TranslateState : AnimationState
    {
        public TranslationKeyFrame[] keyFrames;
        public Transform transform;

        [NonSerialized]
        Vector3 startPos;

        [NonSerialized]
        Vector3 lastPos;

        [NonSerialized]
        int lastKeyFrame;

        public override void Initialize()
        {
            startPos = transform.localPosition;
        }

        public override AnimationKeyFrame[] KeyFrames
        {
            get
            {
                return keyFrames;
            }
        }

        public override void Evaluate(int frame)
        {
            if (frame == 0) return;

            var currentAnimationKeyFrame = keyFrames[keyIndex];

            if (frame > currentAnimationKeyFrame.frameIndex)
            {
                lastPos = currentAnimationKeyFrame.endPos;
                lastKeyFrame = currentAnimationKeyFrame.frameIndex;
                currentKeyFrameIndex = GetNextKeyFrame(frame).frameIndex;
            }
            float time = Mathf.Clamp01( (((float)(frame - lastKeyFrame)) / (currentAnimationKeyFrame.frameIndex - lastKeyFrame)));
            transform.localPosition = Vector3.Lerp(lastPos, currentAnimationKeyFrame.endPos, currentAnimationKeyFrame.curve.Evaluate(time) );
        }

        public override void Restart()
        {
            transform.localPosition = startPos;
            lastPos = startPos;
            currentKeyFrameIndex = 0;
            keyIndex = 0;
            lastKeyFrame = 0;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnimationObject))]
    public class AnimationObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (AnimationObject)target;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Frames Length: ");
            script.frames = EditorGUILayout.IntField(script.frames);
            EditorGUILayout.EndHorizontal();
        }
    }
 #endif

}
