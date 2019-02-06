using System.Collections;
using UnityEngine;

public class CharacterPhoton : Photon.MonoBehaviour
{
    Rigidbody rb;
    Character character;
    CharacterMovement movement;
    Vector3 photonPos;
    Quaternion photonRot;
    Vector3 velo;
    bool attack;
    bool isMaster;

    private void Awake()
    {
        character = GetComponent<Character>();
        movement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (PhotonManager.IsMultiplayer)
        {
            Initialize();
        }
        else
        {
            enabled = false;
        }
    }

    private void OnDestroy()
    {
        PhotonManager.MessageReceived -= AttackEventListner;

    }

    private void OnTriggerEnter(Collider other)
    {
        movement.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        movement.OnTriggerExit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        movement.OnTriggerStay(other);
    }

    void Initialize()
    {
        if (!character.IsLocalPlayer)
        {
            photonPos = transform.position;
            velo = rb.velocity;
            photonRot = transform.rotation;
            isMaster = PhotonManager.IsMaster;
            StartCoroutine(HandlePhotonObject());
        }


        PhotonManager.MessageReceived += AttackEventListner;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
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

    private void AttackEventListner(byte code, int networkingID, object content)
    {

        if (networkingID == character.networking.viewID)
        {
            Debug.Log("Raised Attack");
            if (code == PhotonEventCode.ATTACK)
            {
                movement.Attack();
            }
        }
    }


    IEnumerator HandlePhotonObject()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, photonPos, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, photonRot, 0.1f);
            movement.SetAnimationHorizontal(velo);
            yield return null;
        }
    }
}