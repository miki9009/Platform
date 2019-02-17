using LPWAsset;
using UnityEngine;

public class CharacterFloatingObject : FloatingObject
{
    
    public override void UpdatePosition(float deltaTime, float lerpSpeed)
    {
        return;
    }


    private void FixedUpdate()
    {
        base.UpdatePosition(Time.fixedDeltaTime, manager.lerpSpeed);
    }

}