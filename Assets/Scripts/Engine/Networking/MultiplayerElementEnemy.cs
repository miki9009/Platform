﻿using UnityEngine;

public class MultiplayerElementEnemy : MultiplayerElement
{
    public Animator anim;
    AnimatorClipInfo[] currentClipInfo;
    string currentClipName;

    protected override void Awake()
    {
        base.Awake();
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
    }

    protected override void PhotonManager_MessageReceived(byte code, int id, object content)
    {
        if (code == PhotonEventCode.MULTIPLAYERELEMENT && id == ID)
        {
            var objects = (object[])content;
            lastRecievedPos = (Vector3)objects[0];
            lastRecievedRot = (Quaternion)objects[1];
            var recievedClip = (string)objects[2];
            if (currentClipName !=  recievedClip)
            {
                anim.Play(recievedClip);
                currentClipName = recievedClip;
            }
        }
    }

    public override void SendContent()
    {
        currentClipName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        SendMultiplayerMessage(PhotonEventCode.MULTIPLAYERELEMENT, new object[] { transform.position, transform.rotation,  currentClipName});
    }

}