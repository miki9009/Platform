using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Animations;

namespace Engine
{

    public class AnimatorBehaviour : StateMachineBehaviour
    {
        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateMove is called before OnStateMove is called on any state inside this state machine
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called before OnStateIK is called on any state inside this state machine
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateMachineEnter is called when entering a statemachine via its Entry Node
        //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
        //
        //}

        // OnStateMachineExit is called when exiting a statemachine via its Exit Node
        //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        //
        //}
        public event Action<AnimatorStateInfo> StateEnter;
        public event Action StateUpdate;
        public event Action<AnimatorStateInfo> StateExit;
        public event Action StateMove;
        public event Action StateIK;
        public event Action StateMachineEnter;
        public event Action StateMachineExit;

        [HideInInspector]public GameObject gameObject;
        public IStateAnimator state;
        bool initialized = false;

        

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!initialized)
            {
                gameObject = animator.gameObject;
                state = gameObject.GetComponent<IStateAnimator>();
                if (state == null)
                {
                    state = gameObject.GetComponentInParent<IStateAnimator>();
                }
                if (state == null)
                {
                    state = gameObject.GetComponentInChildren<IStateAnimator>();
                }
                if (state == null)
                {
                    Debug.Log("IStateAnimator not found on this GameObject.");
                }
                else
                {
                    state.AnimatorBehaviour = this;
                    state.StateAnimatorInitialized();
                }
                initialized = true;
            }
            if (StateEnter != null)
            {
                StateEnter(animatorStateInfo);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (StateExit != null)
            {
                StateExit(stateInfo);
            }
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (StateIK != null)
            {
                StateIK();
            }
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (StateMachineEnter != null)
            {
                StateMachineEnter();
            }
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            if (StateMachineExit != null)
            {
                StateMachineExit();
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (StateMove != null)
            {
                StateMove();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (StateUpdate != null)
            {
                StateUpdate();
            }
        }
    }

    public interface IStateAnimator
    {
        /// <summary>
        /// AnimatorBehaviour from attached Animator
        /// </summary>
        AnimatorBehaviour AnimatorBehaviour { get; set; }

        /// <summary>
        /// Method is invoked automaticaly when AnimatorStateMachine is Ready
        /// </summary>
        void StateAnimatorInitialized();
    }

}