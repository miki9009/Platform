using UnityEngine;

public class ActiveObject : MonoBehaviour
{ 
    public bool DeactivatedByManager { get; set; }
    public RealtimeAppearance Manager {get;set;}
   [HideInInspector] public Vector3 scale;
    private void Awake()
    {
        scale = transform.localScale;
    }

    private void OnDisable()
    {
        if (!DeactivatedByManager)
        {
            Manager.objects.Remove(this);
        }
    }

    private void OnEnable()
    {
        DeactivatedByManager = false;
    }
}

