using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshPainter : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));

    }

    public Vector3 GetMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit groundHit;

        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(groundHit.point);
            return groundHit.point;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
}
