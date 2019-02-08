using UnityEngine;
using Engine;
using System.Collections;

[RequireComponent(typeof(LevelElement))]
public class MultiplayerEventElement : MultiplayerObject, IMultiplayerElement
{

    public float messagesInterval = 1;
    private Vector3 lastRecievedPos;
    private Quaternion lastRecievedRot;
    private float lerpSpeed = 10;

    protected override void OnMultiplayerAwake()
    {
        lastRecievedPos = transform.position;
        lastRecievedRot = transform.rotation;
    }


    protected override void OnWillDestroy()
    {

        if(_isMultiplayer)
        {
            PhotonManager.MessageReceived -= PhotonManager_MessageReceived;
        }
    }

    protected override void OnMultiplayerInitialize()
    {
        if(!_isRemote)
            StartCoroutine(Sending());
    }

    protected virtual void PhotonManager_MessageReceived(byte code, int id, object content)
    {
        if(code == PhotonEventCode.MULTIPLAYERELEMENT && id == ID)
        {
            var objects = (object[])content;
            lastRecievedPos = (Vector3)objects[0];
            lastRecievedRot = (Quaternion)objects[1];
        }
    }

    public virtual void SendContent()
    {
        SendMultiplayerMessage(PhotonEventCode.MULTIPLAYERELEMENT, new object[] { transform.position, transform.rotation });
    }

    public void SendMultiplayerMessage(byte code, object content)
    {
        PhotonManager.SendMessage(code, id, content);
    }

    void Update()
    {
        if(IsRemote)
        {
            transform.position = Vector3.Lerp(transform.position, lastRecievedPos, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, lastRecievedRot, Time.deltaTime * lerpSpeed);
        }
    }

    IEnumerator Sending()
    {
        while(true)
        {
            SendContent();
            yield return new WaitForSeconds(messagesInterval);
        }

    }
}

