using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructible
{
    void Hit(CharacterMovement character);
    Transform Transform { get;}
    Rigidbody Rigidbody { get; }
}
