
using UnityEngine;

public class MultiplayerSerializeEnemy : MultiplayerSerializeElement
{
    protected override void OnMultiplayerAwake()
    {
        base.OnMultiplayerAwake();
        Objects = new object[2];
        Objects[0] = transform.position;
        Objects[1] = transform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Serialize");
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            Objects[0] = (Vector3)stream.ReceiveNext();
            Objects[1] = stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (!IsRemote) return;
        transform.position = Vector3.Lerp(transform.position, (Vector3)Objects[0], 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, (Quaternion)Objects[1], 0.1f);
    }
}