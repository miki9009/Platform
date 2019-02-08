using UnityEngine;

public abstract class MultiplayerObject : MonoBehaviour
{
    public MonoBehaviour[] componentsToDisable;

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
    }


}

public interface IMultiplayerElement
{
    int ID { get; }
    bool IsRemote { get; }
    bool IsMultiplayer { get; }
    void SendMultiplayerMessage(byte code, object content);
}