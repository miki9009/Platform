using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public AnimationClip[] animations;

    [HideInInspector]public int animationIndex = -1;
    private int previousAnimationIndex = -1;

    private Animator anim;
    Character character;
    CharacterMovement movement;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        character = GetComponent<Character>();
    }

    // Use this for initialization
    void Start ()
    {
        movement = character.movement;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Animation();
	}

    void Animation()
    {
        var velo = character.rb.velocity;
        anim.SetFloat("vSpeed", velo.y);
        anim.SetFloat("hSpeed", Mathf.Abs(velo.x));
        anim.SetBool("onGround", movement.onGround);
    }
}
