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
        if (!PhotonManager.IsMultiplayer)
            return;
        _isMultiplayer = true;
        Level.LevelLoaded += Initialize;
    }

    private void OnDestroy()
    {
        Level.LevelLoaded -= Initialize;
        if(_isMultiplayer)
        {
            PhotonManager.MessageReceived -= PhotonManager_MessageReceived;
        }
    }

    private void Initialize()
    {
        id = GetComponent<LevelElement>().elementID;
        _isMultiplayer = PhotonManager.IsMultiplayer;
        _isRemote = _isMultiplayer && !PhotonManager.IsMaster;
        if(_isRemote)
            PhotonManager.MessageReceived += PhotonManager_MessageReceived;
        Debug.Log("Is remote: " +_isRemote);
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