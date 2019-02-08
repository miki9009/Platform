using UnityEngine;

using Engine;

public class MultiplayerPathMovement : MultiplayerEventElement
{
    PathMovement path;
    public float timeMessageSend = 1;
    float curTime = 0;
    private void Start()
    {
        path = GetComponent<PathMovement>();
    }


    protected override void PhotonManager_MessageReceived(byte code, int id, object content)
    {
        if(id == this.ID)
        {
            var objs = (object[])content;
            path.pathPoints = (Vector3[])objs[0];
            lastRecievedPos = (Vector3)objs[1];
            Debug.Log("Recieving path ID: " + id);
        }
    }

    private void Update()
    {
        if(IsRemote)          
            transform.position = Vector3.Lerp(transform.position, lastRecievedPos,Time.deltaTime);
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;
        }
        else
        {
            curTime = timeMessageSend;

            if (IsMultiplayer && !IsRemote)
            {
                SendMultiplayerMessage(PhotonEventCode.MULTIPLAYERELEMENT, new object[] { path.pathPoints, transform.position });
                Debug.Log("Sending path ID: " + ID);
            }
        }
    }
}