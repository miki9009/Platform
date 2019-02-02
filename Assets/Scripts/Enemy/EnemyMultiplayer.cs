using UnityEngine;
using Engine;
using System.Collections;

public class EnemyMultiplayer : Photon.MonoBehaviour
{
    bool initialized = false;
    Rigidbody rb;
    Vector3 photonPos;
    Quaternion photonRot;
    Vector3 velo;
    Enemy enemy;

    private void Awake()
    {
        Level.LevelLoaded += Init;
    }

    private void OnDestroy()
    {
        
    }

    void Init()
    {

        if (PhotonManager.IsMultiplayer)
        {
            rb = GetComponent<Rigidbody>();
            photonPos = transform.position;
            velo = rb.velocity;
            photonRot = transform.rotation;
            //PhotonManager.MessageReceived += AttackEventListner;
            initialized = true;
            enemy = GetComponent<Enemy>();
            enemy.enabled = false;
            if (PhotonManager.IsMaster)
            {
                StartCoroutine(HandlePhotonObject());
            }
        }
    }

    void InitMasterClient()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!initialized) return;

        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (rb != null)
                stream.SendNext(rb.velocity);
            else
                stream.SendNext(Vector3.zero);
        }
        else
        {
            photonPos = (Vector3)stream.ReceiveNext();
            photonRot = (Quaternion)stream.ReceiveNext();
            velo = (Vector3)stream.ReceiveNext();
        }
    }

    IEnumerator HandlePhotonObject()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, photonPos, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, photonRot, 0.1f);
            yield return null;
        }
    }
}