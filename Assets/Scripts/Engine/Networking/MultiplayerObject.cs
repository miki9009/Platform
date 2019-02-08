using Engine;
using UnityEngine;

[RequireComponent(typeof(LevelElement))]
public abstract class MultiplayerObject : Photon.MonoBehaviour
{
    public MonoBehaviour[] componentsToDisable;
    public LevelElement levelElement;

    public bool _isMultiplayer;

    public int ID
    {
        get
        {
            return levelElement.elementID;
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



    protected abstract void OnWillDestroy();
    protected abstract void OnMultiplayerInitialize();
    protected abstract void OnMultiplayerAwake();

    private void Awake()
    {
        if (!GameManager.GameMode.Contains("Multi"))
        {
            enabled = false;
            _isRemote = false;
            _isMultiplayer = false;
            return;
        }

        if(!levelElement) levelElement = GetComponent<LevelElement>();

        _isMultiplayer = true;
        PhotonManager.MultiplayerInitialized += Initialize;

        OnMultiplayerAwake();
    }

    private void OnDestroy()
    {
        OnWillDestroy();
        PhotonManager.MultiplayerInitialized -= Initialize;
    }

    private void Initialize()
    {
        _isMultiplayer = PhotonManager.IsMultiplayer;
        _isRemote = _isMultiplayer && !PhotonManager.IsMaster;
        if (_isRemote)
        {
            foreach (var component in componentsToDisable)
            {
                component.enabled = false;
            }
        }
        OnMultiplayerInitialize();
    }


}

public interface IMultiplayerElement
{
    int ID { get; }
    bool IsRemote { get; }
    bool IsMultiplayer { get; }
    void SendMultiplayerMessage(byte code, object content);
}