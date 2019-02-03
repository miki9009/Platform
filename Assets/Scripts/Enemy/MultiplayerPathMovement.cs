using UnityEngine;

using Engine;

public class MultiplayerPathMovement : MultiplayerElement
{
    PathMovement path;
    private void Start()
    {
        path = GetComponent<PathMovement>();
    }

    protected override void PhotonManager_MessageReceived(byte code, int id, object content)
    {
        if(id == this.id)
        {
            path.pathPoints = (Vector3[])content;
            Debug.Log("Recieving path ID: " + id);
        }
    }
}