using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructible
{
    void Hit(IDestructible attacker);
    void CallShake();
    Transform Transform { get;}
    Rigidbody Rigidbody { get; }
    bool Destroyed { get; }
}
