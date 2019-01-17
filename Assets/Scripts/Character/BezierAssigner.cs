using UnityEngine;

public class BezierAssigner : MonoBehaviour
{
    [System.NonSerialized]
    public BezierCurve curve;
    [Range(0.001f,1)]
    public float speed = 0.01f;

    void Awake()
    {
        curve = GetComponent<BezierCurve>();
        Character.CharacterCreated += Assign;
    }

    private void OnDestroy()
    {
        Character.CharacterCreated -= Assign;
    }

    void Assign(Character character)
    {
        //character.movement.bezierAssigner = this;
    }
}