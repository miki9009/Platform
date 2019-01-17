using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Checkpoint : MonoBehaviour
{
    bool activated;
    public TriggerBroadcast checkpointTrigger;
    public TriggerBroadcast particleTrigger;
    public ParticleSystem parts;
    public Transform restartPosition;
    private static List<Checkpoint> checkpoints = new List<Checkpoint>();

    private void Awake()
    {
        checkpoints.Add(this);
        checkpointTrigger.TriggerEntered += CheckpointTrigger;
        particleTrigger.TriggerEntered += ParticleEnter; 
    }

    void CheckpointTrigger(Collider other)
    {
        if (!activated)
        {
            checkpoints.ForEach(x => x.activated = false);
            parts.Play();
            activated = true;
            Controller.Instance.LastCheckpoint = this;
        }
    }

    void ParticleEnter(Collider other)
    {
        if(activated)
        parts.Play();
    }

    void ParticleExit(Collider other)
    {
        parts.Stop();
    }

    public void ResetToCheckpoint(Character character)
    {
        character.transform.position = restartPosition.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(restartPosition.position, 0.5f);
    }
#endif
}