
using UnityEngine;

public class MultiplayerSerializeEnemy : MultiplayerSerializeElement
{
    protected override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
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
        transform.position = Vector3.Lerp(transform.position, (Vector3)Objects[0], 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, (Quaternion)Objects[1], 0.1f);
    }
}