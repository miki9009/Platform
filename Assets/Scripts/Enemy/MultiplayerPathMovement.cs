using UnityEngine;

using Engine;

public class MultiplayerPathMovement : MultiplayerElement
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
        if(id == this.id)
        {
            var objs = (object[])content;
            path.pathPoints = (Vector3[])objs[0];
            transform.position = (Vector3)objs[1];
            Debug.Log("Recieving path ID: " + id);
        }
    }

    private void Update()
    {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;
        }
        else
        {
            curTime = timeMessageSend;

            if (IsMultiplayer && !IsRemote)
            {
                SendMultiplayerMessage(PhotonEventCode.AI_PATH, new object[] { path.pathPoints, transform.position });
                Debug.Log("Sending path ID: " + ID);
            }
        }
    }
}