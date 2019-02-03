using UnityEngine;
using Engine;
using System.Collections;

[RequireComponent(typeof(LevelElement))]
public class MultiplayerElement : MonoBehaviour, IMultiplayerElement
{
    public MonoBehaviour[] componentsToDisable;
    public float messagesInterval = 1;

    public bool _isMultiplayer;
    public int id;
    public float lerpSpeed = 2;
    protected Vector3 lastRecievedPos;
    protected Quaternion lastRecievedRot;

    public int ID
    {
        get
        {
            return id;
        }
    }

    public bool _isRemote;
    public bool IsRemote
    {
        get
        {
            return _isRemote;
        }
    }
    public bool IsMultiplayer
    {
        get
        {
            return _isMultiplayer;
        }
    }

    protected virtual void Awake()
    {
        if (!GameManager.GameMode.Contains("Multi"))
        {
            enabled = false;
            _isRemote = false;
            _isMultiplayer = false;
            return;
        }

        _isMultiplayer = true;
        PhotonManager.MultiplayerInitialized += Initialize;
    }

    private void OnDestroy()
    {
        PhotonManager.MultiplayerInitialized -= Initialize;
        if(_isMultiplayer)
        {
            PhotonManager.MessageReceived -= PhotonManager_MessageReceived;
        }
    }

    private void Initialize()
    {
        Debug.Log("Initialized Multiplayer ");
        id = GetComponent<LevelElement>().elementID;
        _isMultiplayer = PhotonManager.IsMultiplayer;
        _isRemote = _isMultiplayer && !PhotonManager.IsMaster;
        if(_isRemote)
        {
            foreach (var component in componentsToDisable)
            {
                component.enabled = false;
            }
            PhotonManager.MessageReceived += PhotonManager_MessageReceived;
        }
        else
        {
            StartCoroutine(Sending());
        }
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
            Debug.Log("Sending path ID: " + ID);
            SendMultiplayerMessage(PhotonEventCode.MULTIPLAYERELEMENT, new object[] { transform.position, transform.rotation });
            yield return new WaitForSeconds(messagesInterval);
        }

    }
}

public interface IMultiplayerElement
{
    int ID { get; }
    bool IsRemote { get;  }
    bool IsMultiplayer { get; }
    void SendMultiplayerMessage(byte code, object content);
}