using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Animations;
using Engine;


public class CharacterAnimatorBehaviour : AnimatorBehaviour
{

    private float _time;
    public float PlaybackTime
    {
        get
        {
            return _time % 1;
        }
    }

    int _currentStateFullHash;
    public int CurrentStateFullHash
    {
        get { return _currentStateFullHash; }
    }
        

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _time = stateInfo.normalizedTime;
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        _currentStateFullHash = animatorStateInfo.fullPathHash;
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
    }
}
