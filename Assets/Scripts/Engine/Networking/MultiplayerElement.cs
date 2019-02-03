using UnityEngine;
using Engine;

[RequireComponent(typeof(LevelElement))]
public abstract class MultiplayerElement : MonoBehaviour, IMultiplayerElement
{
    public bool _isMultiplayer;
    public int id;

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
            PhotonManager.MessageReceived += PhotonManager_MessageReceived;
    }

    protected abstract void PhotonManager_MessageReceived(byte code, int id, object content);


    public void SendMultiplayerMessage(byte code, object content)
    {
        PhotonManager.SendMessage(code, id, content);
    }
}

public interface IMultiplayerElement
{
    int ID { get; }
    bool IsRemote { get;  }
    bool IsMultiplayer { get; }
    void SendMultiplayerMessage(byte code, object content);
}